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

        using (stream)
        {
            if (VerifierSettings.HasStreamConverter(extension))
            {
                var (newInfo, converted, cleanup) = await DoExtensionConversion(extension, stream, info, null);

                return await VerifyInner(newInfo, cleanup, converted, false);
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

    //TODO: possibly pass in the target here
    async Task<(object? info, List<Target> targets, Func<Task> cleanup)> DoExtensionConversion(string extension, Stream stream, object? info, string? name)
    {
        var cleanup = stream.DisposeAsyncEx;
        var infos = new List<object>();
        if (info != null)
        {
            infos.Add(info);
        }

        var targets = new List<Target>();

        var queue = new Queue<Target>();
        queue.Enqueue(new(extension, stream, name));

        while (queue.Count > 0)
        {
            var target = queue.Dequeue();

            if (!VerifierSettings.TryGetStreamConverter(target.Extension, out var conversion))
            {
                targets.Add(target);
                continue;
            }

            var targetStream = target.StreamData;
            var result = await conversion(target.Name, targetStream, settings.Context);
            if (result.Cleanup != null)
            {
                cleanup += result.Cleanup;
            }
            if (result.Info != null)
            {
                infos.Add(result.Info);
            }

            var resultTargets = result.Targets.ToList();

            foreach (var resultTarget in resultTargets)
            {
                // if the same extension is returned. no need to re process
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
        return (newInfo, targets, cleanup);
    }
}