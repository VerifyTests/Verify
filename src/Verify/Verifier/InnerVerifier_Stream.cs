partial class InnerVerifier
{
    public Task<VerifyResult> VerifyStream(FileStream? stream)
    {
        if (stream is null)
        {
            return VerifyInner(emptyTargets);
        }

        return VerifyStream(stream, EmptyFiles.Extensions.GetExtension(stream.Name));
    }

    public async Task<VerifyResult> VerifyStream(Task<FileStream> task)
    {
        var stream = await task;
        return await VerifyStream(stream);
    }

    public Task<VerifyResult> VerifyStream(byte[]? bytes, string extension)
    {
        if (bytes is null)
        {
            return VerifyInner(emptyTargets);
        }

        return VerifyStream(new MemoryStream(bytes), extension);
    }

    public async Task<VerifyResult> VerifyStream(Task<byte[]> task, string extension)
    {
        var bytes = await task;
        return await VerifyStream(bytes, extension);
    }

    public async Task<VerifyResult> VerifyStream<T>(Task<T> task, string extension)
        where T : Stream
    {
        var stream = await task;
        return await VerifyStream(stream, extension);
    }

    public async Task<VerifyResult> VerifyStreams<T>(IEnumerable<T> streams, string extension)
        where T : Stream
    {
        var targets = streams.Select(_ => new Target(extension, _));
        return await VerifyInner(targets);
    }

    public async Task<VerifyResult> VerifyStream(Stream? stream, string extension)
    {
        if (stream is null)
        {
            return await VerifyInner(emptyTargets);
        }

        using (stream)
        {
            if (stream.Length == 0)
            {
                throw new("Empty data is not allowed.");
            }

            if (VerifierSettings.HasExtensionConverter(extension))
            {
                var (info, converted, cleanup) = await DoExtensionConversion(extension, stream);

                return await VerifyInner(info, cleanup, converted);
            }

            var target = await GetTarget(stream, extension);

            return await VerifyInner(
                new[]
                {
                    target
                });
        }
    }

    static async Task<Target> GetTarget(Stream stream, string extension)
    {
        if (EmptyFiles.Extensions.IsText(extension))
        {
            return new(extension, await stream.ReadString());
        }

        return new(extension, stream);
    }

    async Task<(object? info, List<Target> targets, Func<Task> cleanup)> DoExtensionConversion(string extension, Stream stream)
    {
        Func<Task> cleanup = stream.DisposeAsyncEx;
        var infos = new List<object>();
        var targets = new List<Target>();

        var queue = new Queue<Target>();
        queue.Enqueue(new(extension, stream));

        while (queue.Count > 0)
        {
            var target = queue.Dequeue();

            if (!VerifierSettings.TryGetExtensionConverter(target.Extension, out var conversion))
            {
                targets.Add(target);
                continue;
            }

            var result = await conversion(target.StreamData, settings.Context);
            if (result.Info != null)
            {
                infos.Add(result.Info);
            }

            if (result.Cleanup != null)
            {
                cleanup += result.Cleanup;
            }

            queue.Enqueue(result.Targets);
        }

        var info = infos.Count switch
        {
            1 => infos[0],
            > 1 => infos,
            _ => null
        };
        return (info, targets, cleanup);
    }
}