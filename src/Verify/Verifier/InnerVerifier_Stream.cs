namespace VerifyTests;

partial class InnerVerifier
{
    public Task<VerifyResult> VerifyStream(FileStream? stream, object? info)
    {
        if (stream is null)
        {
            if (info == null)
            {
                return VerifyInner(emptyTargets);
            }

            return Verify(info);
        }

        return VerifyStream(stream, stream.Extension(), info);
    }

    public Task<VerifyResult> VerifyStream(byte[]? bytes, object? info) =>
        VerifyStream(bytes, "bin", info);

    public Task<VerifyResult> VerifyStream(byte[]? bytes, string extension, object? info)
    {
        if (bytes is null)
        {
            if (info == null)
            {
                return VerifyInner(emptyTargets);
            }

            return Verify(info);
        }

        return VerifyStream(new MemoryStream(bytes), extension, info);
    }

    public async Task<VerifyResult> VerifyStream(Task<byte[]> task, string extension, object? info) =>
        await VerifyStream(await task, extension, info);

    public async Task<VerifyResult> VerifyStream(ValueTask<byte[]> task, string extension, object? info) =>
        await VerifyStream(await task, extension, info);

    public async Task<VerifyResult> VerifyStream<T>(Task<T> task, string extension, object? info)
        where T : Stream =>
        await VerifyStream(await task, extension, info);

    public async Task<VerifyResult> VerifyStream<T>(ValueTask<T> task, string extension, object? info)
        where T : Stream =>
        await VerifyStream(await task, extension, info);

    public Task<VerifyResult> VerifyStreams<T>(IEnumerable<T> streams, string extension, object? info)
        where T : Stream
    {
        var targets = streams
            .Select(_ => new Target(extension, _))
            .ToList();

        if (info is not null)
        {
            targets.Insert(
                0,
                new(
                    settings.TxtOrJson,
                    JsonFormatter.AsJson(settings, counter, info)));
        }

        return VerifyInner(targets);
    }

    public Task<VerifyResult> VerifyStream(Stream? stream, object? info) =>
        VerifyStream(stream, "bin", info);

    public async Task<VerifyResult> VerifyStream(Stream? stream, string extension, object? info)
    {
        Guards.AgainstBadExtension(extension);
        if (stream is null)
        {
            if (info is null)
            {
                return await VerifyInner(emptyTargets);
            }

            return await Verify(info);
        }

        stream.MoveToStart();

        // ReSharper disable once UseAwaitUsing
        using (stream)
        {
            if (VerifierSettings.HasStreamConverter(extension))
            {
                var initial = await GetTarget(stream, extension);
                var converted = await DoExtensionConversion(initial, info);

                return await VerifyInner(converted.Info, converted.Cleanup, converted.Targets, false, true, converted.InfoOwner, converted.InfoIsOfDocument);
            }

            var target = await GetTarget(stream, extension);

            var targets = new List<Target>(1);

            if (info is not null)
            {
                targets.Add(
                    new(
                        settings.TxtOrJson,
                        JsonFormatter.AsJson(settings, counter, info)));
            }

            targets.Add(target);
            return await VerifyInner(targets);
        }
    }

    static async Task<Target> GetTarget(Stream stream, string extension)
    {
        if (FileExtensions.IsTextExtension(extension))
        {
            return new(extension, await stream.ReadStringBuilderWithFixedLines());
        }

        return new(extension, stream);
    }

    /// <summary>
    /// What converting one target, and whatever its conversion produced that could itself be
    /// converted, came to.
    /// </summary>
    /// <param name="Info">The infos of the conversions, with the one passed in first.</param>
    /// <param name="Targets">The targets left once nothing more could be converted.</param>
    /// <param name="Cleanup">Disposes what the conversions own.</param>
    /// <param name="InfoOwner">
    /// The conversion <paramref name="Info" /> belongs to, where it is nothing but what converters
    /// said of a source the first of them named. Its info file is then derived from that source.
    /// </param>
    /// <param name="InfoName">
    /// The name of the info file, where the first conversion names its targets relative to the
    /// target it converted: that target's name.
    /// </param>
    /// <param name="InfoIsOfDocument">
    /// <paramref name="Info" /> is nothing but what converters said of a document the first of
    /// them told apart from what it derived, whether or not that document is itself a target. It
    /// is then a file of the document, and stays one: see <see cref="Target.DontInline" />.
    /// </param>
    readonly record struct Converted(object? Info, List<Target> Targets, Func<Task> Cleanup, ConversionToken? InfoOwner, string? InfoName, bool InfoIsOfDocument);

    // A converter ran for this verification. A converter that is told what is excluded leaves
    // those targets out itself, so having none left can be the exclusions' doing with nothing
    // having been removed here: see VerifyInner
    bool conversionRan;

    async Task<Converted> DoExtensionConversion(Target initial, object? info)
    {
        var cleanup = () => Task.CompletedTask;
        // the source stream of a stream target is owned here, so dispose it once consumed
        if (initial.IsStream)
        {
            cleanup = cleanup.Then(initial.StreamData.DisposeAsyncEx);
        }

        var infos = new List<object>();
        if (info != null)
        {
            infos.Add(info);
        }

        var targets = new List<Target>();
        ConversionToken? infoOwner = null;
        string? infoName = null;
        var infoIsOfDocument = false;
        var isInitial = true;

        var queue = new Queue<Target>();
        queue.Enqueue(initial);

        while (queue.Count > 0)
        {
            var target = queue.Dequeue();

            // A source is the document as its conversion gave it, so it is not converted again
            // whatever its extension: a doc given back as a docx is not then run through the docx
            // converter. PerformConversion is how any other target asks for the same
            if (target.IsSource ||
                !target.PerformConversion ||
                !VerifierSettings.TryGetStreamConverter(target.Extension, out var conversion))
            {
                // terminal target: scrub text before it is finalized
                Scrub(target);
                targets.Add(target);
                continue;
            }

            // scrub text before conversion so derived targets (eg rendered images) reflect the scrubbed content
            Scrub(target);

            Stream targetStream;
            if (target.IsStream)
            {
                targetStream = target.StreamData;
            }
            else
            {
                // a text target is fed to the converter as a utf8 stream
                target.TryGetStringBuilder(out var builder);
                var memory = new MemoryStream(Encoding.UTF8.GetBytes(builder!.ToString()));
                cleanup = cleanup.Then(memory.DisposeAsyncEx);
                targetStream = memory;
            }

            var result = await conversion(target.Name, targetStream, settings.Context);
            if (result.Cleanup != null)
            {
                cleanup = cleanup.Then(result.Cleanup);
            }

            if (result.Info != null)
            {
                infos.Add(result.Info);
            }

            conversionRan = true;
            var resultTargets = Adopt(result, target.Name, target.Conversion, target.IsDerived, out var token);
            if (isInitial)
            {
                isInitial = false;
                // With an info passed in, the file holds that as well, and is not the source's
                if (info is null &&
                    result.Source is not null)
                {
                    infoOwner = token;
                }

                if (result.IsDerivation)
                {
                    infoName = target.Name;
                    infoIsOfDocument = info is null;
                }
            }

            foreach (var resultTarget in resultTargets)
            {
                // if the same extension is returned. no need to re process.
                // its content derives from the already scrubbed input, so it is not scrubbed again
                if (resultTarget.Extension == target.Extension)
                {
                    targets.Add(resultTarget);
                }
                else
                {
                    queue.Enqueue(resultTarget);
                }
            }
        }

        var newInfo = infos.Count switch
        {
            1 => infos[0],
            > 1 => infos,
            _ => null
        };
        return new(newInfo, targets, cleanup, infoOwner, infoName, infoIsOfDocument);
    }

    /// <summary>
    /// Takes what a conversion returned into the verification. The one place a
    /// <see cref="ConversionToken" /> is made, and where the targets of a conversion that told
    /// its source from what it derived are named and tied to that source.
    /// </summary>
    /// <param name="result">What the conversion returned.</param>
    /// <param name="name">The name of the target that was converted, or null.</param>
    /// <param name="parent">The conversion that target came out of, or null.</param>
    /// <param name="derived">Whether that target was itself derived from a document.</param>
    /// <param name="token">
    /// The conversion the targets belong to: a new one where <paramref name="result" /> names a
    /// source, and otherwise <paramref name="parent" />, since what was computed from a derived
    /// target was derived from the same source it was.
    /// </param>
    static List<Target> Adopt(in ConversionResult result, string? name, ConversionToken? parent, bool derived, out ConversionToken? token)
    {
        if (!result.IsDerivation)
        {
            // Named by the converter, which was passed the name to do it with
            token = parent;
            if (!derived)
            {
                return result.Targets.ToList();
            }

            // Computed from a derived target, so derived from whatever that was. Its source where
            // it has one, and derived all the same where the source was left out
            return result.Targets
                .Select(_ => _ with { Conversion = parent, IsDerived = true })
                .ToList();
        }

        var hasSource = result.Source is not null;
        if (hasSource)
        {
            token = new(parent);
        }
        else
        {
            token = parent;
        }

        var targets = new List<Target>();
        foreach (var target in result.Targets)
        {
            // The source is the first of the targets where there is one
            var isSource = hasSource && targets.Count == 0;
            targets.Add(
                target with
                {
                    Name = RelativeName(name, target.Name),
                    Conversion = token,
                    IsSource = isSource,
                    IsDerived = !isSource
                });
        }

        return targets;
    }

    /// <summary>
    /// A target named <c>page_0001</c>, from the conversion of a target named <c>Attachment1</c>,
    /// is <c>Attachment1.page_0001</c>. Either on its own where the other is missing.
    /// </summary>
    static string? RelativeName(string? converted, string? name)
    {
        if (converted is null)
        {
            return name;
        }

        if (name is null)
        {
            return converted;
        }

        return $"{converted}.{name}";
    }
}