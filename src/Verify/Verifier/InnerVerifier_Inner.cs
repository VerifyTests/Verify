namespace VerifyTests;

partial class InnerVerifier
{
    Task<VerifyResult> VerifyInner(IEnumerable<Target> targets) =>
        VerifyInner(null, null, targets, true, true);

    async Task<VerifyResult> VerifyInner(object? root, Func<Task>? cleanup, IEnumerable<Target> targets, bool doExtensionConversion, bool ignoreNullRoot)
    {
        (cleanup, var resultTargets) = await GetTargets(root, cleanup, targets, doExtensionConversion, ignoreNullRoot);
        var engine = new VerifyEngine(
            directory,
            settings,
            verifiedFiles,
            getFileNames,
            getIndexedFileNames,
            settings.TypeName ?? typeName,
            settings.MethodName ?? methodName);

        await engine.HandleResults(resultTargets);

        await cleanup();

        await engine.ThrowIfRequired();

        var filePairs = new List<FilePair>(engine.Equal);
        if (engine.AutoVerified.Count > 0)
        {
            filePairs.AddRange(engine.AutoVerified);
        }

        return new(filePairs, root);
    }

    async Task<(Func<Task> cleanup, List<ResolvedTarget> resultTargets)> GetTargets(object? root, Func<Task>? cleanup, IEnumerable<Target> targets, bool doExtensionConversion, bool ignoreNullRoot)
    {
        var resultTargets = new List<ResolvedTarget>();
        cleanup ??= () => Task.CompletedTask;

        List<Target> list = [..targets, ..VerifierSettings.GetFileAppenders(settings)];

        // Process root via TryGetRootTarget when root is NOT already in the targets list
        // This handles:
        // - VerifyStream with converter: root (info) is separate from targets → process both
        // - Verify(object, IEnumerable<Target>): root is separate from targets → process both
        // - Verify(): root is null, targets is empty → TryGetRootTarget handles null appropriately
        // Skip when:
        // - Verify(object): root is in targets list → avoid duplicate processing
        var rootInTargets = root != null && list.Any(t => ReferenceEquals(t.Data, root));
        if (!rootInTargets && TryGetRootTarget(root, ignoreNullRoot, out var rootTarget))
        {
            resultTargets.Add(rootTarget.Value);
        }

        if (doExtensionConversion)
        {
            var result = new List<Target>();
            foreach (var target in list)
            {
                if (!target.PerformConversion ||
                    target.Extension is null ||
                    !VerifierSettings.HasStreamConverter(target.Extension))
                {
                    result.Add(target);
                    continue;
                }

                var (info, converted, itemCleanup) = await DoExtensionConversion(target.Extension, target.StreamData, null, target.Name);
                cleanup += itemCleanup;
                if (info != null)
                {
                    result.Add(
                        new(
                            settings.TxtOrJson,
                            JsonFormatter.AsJson(
                                settings,
                                counter,
                                info)));
                }

                result.AddRange(converted);
            }

            list = result;
        }

        // Resolve all targets (including objects) to ResolvedTargets
        var extraTargets = new List<ResolvedTarget>();
        var noScrubTargets = new List<ResolvedTarget>();
        foreach (var target in list)
        {
            var (resolvedList, resolveCleanup, applyScrubbers) = await ResolveTarget(target);
            cleanup += resolveCleanup;
            if (applyScrubbers)
            {
                extraTargets.AddRange(resolvedList);
            }
            else
            {
                noScrubTargets.AddRange(resolvedList);
            }
        }

        // Apply scrubbers to StringBuilder targets (not to root-like targets)
        foreach (var target in extraTargets)
        {
            if (target.TryGetStringBuilder(out var builder))
            {
                ApplyScrubbers.ApplyForExtension(target.Extension, builder, settings, counter);
            }
        }

        resultTargets.AddRange(noScrubTargets);
        resultTargets.AddRange(extraTargets);
        return (cleanup, resultTargets);
    }

    async Task<(List<ResolvedTarget> targets, Func<Task> cleanup, bool applyScrubbers)> ResolveTarget(Target target)
    {
        var cleanup = () => Task.CompletedTask;
        var results = new List<ResolvedTarget>();

        // If target has an extension, it's already resolved (Stream or StringBuilder)
        // These come from converters or explicit targets, so apply scrubbers
        if (target.Extension is not null)
        {
            if (target.TryGetStream(out var stream))
            {
                results.Add(new(target.Extension, stream, target.Name));
            }
            else if (target.TryGetStringBuilder(out var sb))
            {
                results.Add(new(target.Extension, sb, target.Name));
            }
            return (results, cleanup, true);
        }

        // Target contains an arbitrary object - resolve it using full type matching
        var data = target.Data;
        if (data is null)
        {
            // Serialize null to JSON using InfoBuilder - no scrubbing for JSON
            results.Add(new(
                settings.TxtOrJson,
                JsonFormatter.AsJson(settings, counter, new InfoBuilder(false, null, []))));
            return (results, cleanup, false);
        }

        // Handle XContainer (XDocument, XElement) - apply scrubbers
        if (data is XContainer container)
        {
            var xmlString = ConvertXmlToString(container);
            results.Add(new("xml", xmlString, target.Name));
            return (results, cleanup, true);
        }

        // Handle XmlNode - apply scrubbers
        if (data is XmlNode node)
        {
            using var reader = new XmlNodeReader(node);
            reader.MoveToContent();
            var xdoc = XDocument.Load(reader);
            var xmlString = ConvertXmlToString(xdoc);
            results.Add(new("xml", xmlString, target.Name));
            return (results, cleanup, true);
        }

        // Handle FileStream (get extension from filename) - apply scrubbers
        if (data is FileStream fileStream)
        {
            var extension = fileStream.Extension();
            var (streamResults, streamCleanup) = await ResolveStream(fileStream, extension, target.Name);
            return (streamResults, streamCleanup, true);
        }

        // Handle Stream - apply scrubbers
        if (data is Stream stream2)
        {
            var (streamResults, streamCleanup) = await ResolveStream(stream2, "bin", target.Name);
            return (streamResults, streamCleanup, true);
        }

        // Handle byte[] - apply scrubbers
        if (data is byte[] bytes)
        {
            var memStream = new MemoryStream(bytes);
            var (streamResults, streamCleanup) = await ResolveStream(memStream, "bin", target.Name);
            return (streamResults, streamCleanup, true);
        }

        // Handle IEnumerable<Stream> - throw error as in Verify(object)
        if (data is IEnumerable<Stream>)
        {
            throw new("Use Verify(IEnumerable<T> targets, string extension)");
        }

        // Handle StringBuilder - apply scrubbers
        if (data is StringBuilder sb2)
        {
            results.Add(new("txt", sb2, target.Name));
            return (results, cleanup, true);
        }

        // Handle string - check for JSON appenders first (matches TryGetRootTarget behavior)
        if (data is string str)
        {
            str = str.TrimPreamble();
            if (str.Length == 0)
            {
                str = "emptyString";
            }

            var appends = VerifierSettings.GetJsonAppenders(settings);
            if (appends.Count > 0)
            {
                // Format as JSON with appenders - no scrubbing for JSON
                results.Add(new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(settings, counter, new InfoBuilder(false, str, appends))));
                return (results, cleanup, false);
            }

            // Plain text - apply scrubbers
            results.Add(new("txt", str, target.Name));
            return (results, cleanup, true);
        }

        // Try ToString converter (before typed converter, matching Verify(object) order) - apply scrubbers
        if (VerifierSettings.TryGetToString(data, out var toString))
        {
            var stringResult = toString(data, settings.Context);
            var extension = stringResult.Extension ?? "txt";
            results.Add(new(extension, stringResult.Value, target.Name));
            return (results, cleanup, true);
        }

        // Try typed converter - apply scrubbers
        if (VerifierSettings.TryGetTypedConverter(data, settings, out var converter))
        {
            var conversionResult = await converter.Conversion(data, settings.Context);
            if (conversionResult.Cleanup != null)
            {
                cleanup += conversionResult.Cleanup;
            }

            // Recursively resolve the conversion result targets
            foreach (var convTarget in conversionResult.Targets)
            {
                // Note: We ignore the applyScrubbers from recursive calls since converter output should be scrubbed
                var (resolved, resolveCleanup, _) = await ResolveTarget(convTarget);
                cleanup += resolveCleanup;
                results.AddRange(resolved);
            }

            // Add info as a separate target if present
            if (conversionResult.Info != null)
            {
                results.Insert(0, new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(settings, counter, conversionResult.Info)));
            }

            return (results, cleanup, true);
        }

        // Fall back to JSON serialization with appenders - NO scrubbing (matches original TryGetRootTarget behavior)
        var jsonAppends = VerifierSettings.GetJsonAppenders(settings);
        results.Add(new(
            settings.TxtOrJson,
            JsonFormatter.AsJson(settings, counter, new InfoBuilder(true, data, jsonAppends))));
        return (results, cleanup, false);
    }

    async Task<(List<ResolvedTarget> targets, Func<Task> cleanup)> ResolveStream(Stream stream, string extension, string? name)
    {
        var cleanup = () => Task.CompletedTask;
        var results = new List<ResolvedTarget>();

        stream.MoveToStart();

        // Check for stream converter
        if (VerifierSettings.HasStreamConverter(extension))
        {
            var (info, converted, convCleanup) = await DoExtensionConversion(extension, stream, null, name);
            cleanup += convCleanup;

            // Add info target if present
            if (info != null)
            {
                results.Add(new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(settings, counter, info)));
            }

            // Recursively resolve converted targets (they may contain objects)
            foreach (var convTarget in converted)
            {
                var (resolved, resolveCleanup, _) = await ResolveTarget(convTarget);
                cleanup += resolveCleanup;
                results.AddRange(resolved);
            }

            return (results, cleanup);
        }

        // No converter - convert stream directly to ResolvedTarget
        if (FileExtensions.IsTextExtension(extension))
        {
            results.Add(new(extension, await stream.ReadStringBuilderWithFixedLines(), name));
        }
        else
        {
            results.Add(new(extension, stream, name));
        }

        return (results, cleanup);
    }

    string ConvertXmlToString(XContainer target)
    {
        var serialization = settings.serialization;
        var elements = target
            .Descendants()
            .ToList();

        foreach (var element in elements)
        {
            if (serialization.TryGetScrubOrIgnoreByName(element.Name.LocalName, out var scrubOrIgnore))
            {
                if (scrubOrIgnore == ScrubOrIgnore.Ignore)
                {
                    element.Remove();
                }
                else
                {
                    element.Value = "Scrubbed";
                }

                continue;
            }

            ScrubXmlAttributes(element, serialization);
        }

        foreach (var node in target.DescendantNodes())
        {
            switch (node)
            {
                case XText text:
                    text.Value = ConvertXmlValue(text.Value);
                    continue;
                case XComment comment:
                    comment.Value = ConvertXmlValue(comment.Value);
                    continue;
            }
        }

        return target.ToString();
    }

    string ConvertXmlValue(string value)
    {
        var span = value.AsSpan();
        if (counter.TryConvert(span, out var result))
        {
            return result;
        }

        return ApplyScrubbers.ApplyForPropertyValue(span, settings, counter).ToString();
    }

    void ScrubXmlAttributes(XElement node, SerializationSettings serialization)
    {
        foreach (var attribute in node
                     .Attributes()
                     .ToList())
        {
            if (serialization.TryGetScrubOrIgnoreByName(attribute.Name.LocalName, out var scrubOrIgnore))
            {
                if (scrubOrIgnore == ScrubOrIgnore.Ignore)
                {
                    attribute.Remove();
                }
                else
                {
                    attribute.Value = "Scrubbed";
                }

                continue;
            }

            var span = attribute.Value.AsSpan();
            if (counter.TryConvert(span, out var result))
            {
                attribute.Value = result;
            }
            else
            {
                attribute.Value = ApplyScrubbers.ApplyForPropertyValue(span, settings, counter).ToString();
            }
        }
    }

    bool TryGetRootTarget(object? root, bool ignoreNullRoot, [NotNullWhen(true)] out ResolvedTarget? target)
    {
        var appends = VerifierSettings.GetJsonAppenders(settings);

        var hasAppends = appends.Count > 0;

        if (ignoreNullRoot && root == null && !hasAppends)
        {
            target = null;
            return false;
        }

        if (root is string stringRoot)
        {
            stringRoot = stringRoot.TrimPreamble();
            if (stringRoot.Length == 0)
            {
                stringRoot = "emptyString";
            }

            if (hasAppends)
            {
                target = new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(
                        settings,
                        counter,
                        new InfoBuilder(false, stringRoot, appends)));
            }
            else
            {
                var builder = new StringBuilder(stringRoot);
                ApplyScrubbers.ApplyForExtension("txt", builder, settings, counter);
                target = new("txt", builder);
            }

            return true;
        }

        target = new(
            settings.TxtOrJson,
            JsonFormatter.AsJson(
                settings,
                counter,
                new InfoBuilder(ignoreNullRoot, root, appends)));
        return true;
    }
}
