partial class InnerVerifier
{
    public Task<VerifyResult> VerifyStream(FileStream? stream)
    {
        if (stream is null)
        {
            return VerifyInner(null, null, emptyTargets);
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
            return VerifyInner(null, null, emptyTargets);
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
        return await VerifyInner(null, null, targets);
    }

    public async Task<VerifyResult> VerifyStream(Stream? stream, string extension)
    {
        if (stream is null)
        {
            return await VerifyInner(null, null, emptyTargets);
        }

        using (stream)
        {
            if (stream.Length == 0)
            {
                throw new("Empty data is not allowed.");
            }

            if (VerifierSettings.HasExtensionConverter(extension))
            {
                var (infos, convertedTargets, cleanups) = await DoExtensionConversion(
                    new()
                    {
                        new(extension, stream)
                    });
                object? info = null;
                if (infos.Count == 1)
                {
                    info = infos[0];
                }
                else if (infos.Count > 1)
                {
                    info = infos;
                }

                return await VerifyInner(
                    info,
                    async () =>
                    {
                        foreach (var cleanup in cleanups)
                        {
                            await cleanup();
                        }
                    },
                    convertedTargets);
            }

            var targets = await GetTargets(stream, extension);

            return await VerifyInner(null, null, targets);
        }
    }

    static async Task<List<Target>> GetTargets(Stream stream, string extension)
    {
        if (EmptyFiles.Extensions.IsText(extension))
        {
            return new()
            {
                new(extension, await stream.ReadString())
            };
        }

        return new()
        {
            new(extension, stream)
        };
    }

    async Task<(List<object> infos, List<Target> targets, List<Func<Task>> cleanups)> DoExtensionConversion(List<Target> list)
    {
        var infos = new List<object>();
        var outputTargets = new List<Target>();
        var cleanups = new List<Func<Task>>();

        var queue = new Queue<Target>(list);

        while (queue.Count > 0)
        {
            var target = queue.Dequeue();

            if (!VerifierSettings.TryGetExtensionConverter(target.Extension, out var conversion))
            {
                outputTargets.Add(target);
                continue;
            }

            var result = await conversion(target.StreamData, settings.Context);
            if (result.Info != null)
            {
                infos.Add(result.Info);
            }

            if (result.Cleanup != null)
            {
                cleanups.Add(result.Cleanup);
            }

            foreach (var innerTarget in result.Targets)
            {
                queue.Enqueue(innerTarget);
            }
        }

        return (infos, outputTargets, cleanups);
    }
}