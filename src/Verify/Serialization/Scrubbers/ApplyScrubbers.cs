static class ApplyScrubbers
{
    public static void ApplyForExtension(string extension, StringBuilder target, VerifySettings settings, Counter counter)
    {
        if (!settings.ScrubbersEnabled)
        {
            target.FixNewlines();
            return;
        }

        var set = EngineScrubberSet.ForExtension(settings, extension);
        var source = target.ToString();

        if (!HasLegacyForExtension(settings, extension))
        {
            ScrubEngine.RunToBuilder(source, set, counter, settings.Context, applyDirectoryReplacements: true, target);
            return;
        }

        // Span scrubbers run first, then the legacy pass over the intermediate result.
        // Path replacements and newline normalization stay after the legacy pass so
        // legacy scrubbers keep seeing raw paths and may inject '\r'.
        var intermediate = ScrubEngine.Run(source, set, counter, settings.Context, applyDirectoryReplacements: false);
        if (!ReferenceEquals(intermediate, source))
        {
            target.Clear();
            target.Append(intermediate);
        }

        if (settings.InstanceScrubbers != null)
        {
            foreach (var scrubber in settings.InstanceScrubbers)
            {
                scrubber(target, counter, settings.Context);
            }
        }

        if (settings.ExtensionMappedInstanceScrubbers != null &&
            settings.ExtensionMappedInstanceScrubbers.TryGetValue(extension, out var extensionBasedInstanceScrubbers))
        {
            foreach (var scrubber in extensionBasedInstanceScrubbers)
            {
                scrubber(target, counter, settings.Context);
            }
        }

        if (VerifierSettings.ExtensionMappedGlobalScrubbers.TryGetValue(extension, out var extensionBasedScrubbers))
        {
            foreach (var scrubber in extensionBasedScrubbers)
            {
                scrubber(target, counter, settings.Context);
            }
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(target, counter, settings.Context);
        }

        DirectoryReplacements.Replace(target);

        target.FixNewlines();
    }

    // Fast path: when nothing can change the value, skip all scrubbing work
    // (this is the hottest loop in the library — one call per serialized string
    // value). Returns the input span unchanged (zero alloc) when possible.
    public static CharSpan ApplyForPropertyValue(CharSpan value, VerifySettings settings, Counter counter)
    {
        if (!settings.ScrubbersEnabled)
        {
            if (!value.Contains('\r'))
            {
                return value;
            }

            return Scrubber.NormalizeNewlines(value.ToString()).AsSpan();
        }

        if (!HasLegacyForPropertyValue(settings))
        {
            var set = EngineScrubberSet.ForPropertyValue(settings);
            var minTrigger = Math.Min(set.MinTrigger, DirectoryReplacements.ShortestFindLength);
            if (value.Length < minTrigger &&
                !value.Contains('\r'))
            {
                return value;
            }

            return ScrubEngine.Run(value.ToString(), set, counter, settings.Context, applyDirectoryReplacements: true).AsSpan();
        }

        return ApplyWithLegacy(value.ToString(), settings, counter).AsSpan();
    }

    // String variant: returns the same string instance when nothing changed
    public static string ApplyForPropertyValue(string value, VerifySettings settings, Counter counter)
    {
        if (!settings.ScrubbersEnabled)
        {
            return Scrubber.NormalizeNewlines(value);
        }

        if (!HasLegacyForPropertyValue(settings))
        {
            var set = EngineScrubberSet.ForPropertyValue(settings);
            var minTrigger = Math.Min(set.MinTrigger, DirectoryReplacements.ShortestFindLength);
            if (value.Length < minTrigger &&
                !value.Contains('\r'))
            {
                return value;
            }

            return ScrubEngine.Run(value, set, counter, settings.Context, applyDirectoryReplacements: true);
        }

        return ApplyWithLegacy(value, settings, counter);
    }

    static string ApplyWithLegacy(string value, VerifySettings settings, Counter counter)
    {
        var set = EngineScrubberSet.ForPropertyValue(settings);
        var intermediate = ScrubEngine.Run(value, set, counter, settings.Context, applyDirectoryReplacements: false);

        var builder = new StringBuilder(intermediate);
        if (settings.InstanceScrubbers != null)
        {
            foreach (var scrubber in settings.InstanceScrubbers)
            {
                scrubber(builder, counter, settings.Context);
            }
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(builder, counter, settings.Context);
        }

        DirectoryReplacements.Replace(builder);

        builder.FixNewlines();
        return builder.ToString();
    }

    static bool HasLegacyForExtension(VerifySettings settings, string extension) =>
        settings.InstanceScrubbers is { Count: > 0 } ||
        (settings.ExtensionMappedInstanceScrubbers != null &&
         settings.ExtensionMappedInstanceScrubbers.TryGetValue(extension, out var extensionInstance) &&
         extensionInstance.Count > 0) ||
        (VerifierSettings.ExtensionMappedGlobalScrubbers.TryGetValue(extension, out var extensionGlobal) &&
         extensionGlobal.Count > 0) ||
        VerifierSettings.GlobalScrubbers.Count > 0;

    static bool HasLegacyForPropertyValue(VerifySettings settings) =>
        settings.InstanceScrubbers is { Count: > 0 } ||
        VerifierSettings.GlobalScrubbers.Count > 0;
}
