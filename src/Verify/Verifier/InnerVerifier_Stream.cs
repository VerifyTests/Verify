﻿partial class InnerVerifier
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

        return VerifyStream(stream, EmptyFiles.Extensions.GetExtension(stream.Name), info);
    }

    public async Task<VerifyResult> VerifyStream(Task<FileStream> task, object? info)
    {
        var stream = await task;
        return await VerifyStream(stream, info);
    }

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

    public async Task<VerifyResult> VerifyStream(Task<byte[]> task, string extension, object? info)
    {
        var bytes = await task;
        return await VerifyStream(bytes, extension, info);
    }

    public async Task<VerifyResult> VerifyStream<T>(Task<T> task, string extension, object? info)
        where T : Stream
    {
        var stream = await task;
        return await VerifyStream(stream, extension, info);
    }

    public async Task<VerifyResult> VerifyStreams<T>(IEnumerable<T> streams, string extension, object? inf)
        where T : Stream
    {
        var targets = streams.Select(_ => new Target(extension, _));
        return await VerifyInner(targets);
    }

    public async Task<VerifyResult> VerifyStream(Stream? stream, string extension, object? info)
    {
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
            if (stream.Length == 0)
            {
                throw new("Empty data is not allowed.");
            }

            if (VerifierSettings.HasExtensionConverter(extension))
            {
                var (newInfo, converted, cleanup) = await DoExtensionConversion(extension, stream, info);

                return await VerifyInner(newInfo, cleanup, converted, false);
            }

            var target = await GetTarget(stream, extension);

            var targets = new List<Target>();

            if (info is not null)
            {
                targets.Add(
                    new(
                        VerifierSettings.TxtOrJson,
                        JsonFormatter.AsJson(settings, counter, info)));
            }

            targets.Add(target);
            return await VerifyInner(targets);
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

    async Task<(object? info, List<Target> targets, Func<Task> cleanup)> DoExtensionConversion(string extension, Stream stream, object? info)
    {
        Func<Task> cleanup = stream.DisposeAsyncEx;
        var infos = new List<object>();
        if (info != null)
        {
            infos.Add(info);
        }
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

            var targetStream = target.StreamData;
            var result = await conversion(targetStream, settings.Context);
            if (result.Info != null)
            {
                infos.Add(result.Info);
            }

            var resultTargets = result.Targets.ToList();

            // if the same stream is returned. no need to re process
            if (resultTargets.Count == 1)
            {
                var single = resultTargets.Single();
                if (single.IsStream && single.StreamData == targetStream)
                {
                    targets.Add(single);
                    continue;
                }
            }

            if (result.Cleanup != null)
            {
                cleanup += result.Cleanup;
            }

            queue.Enqueue(resultTargets);
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