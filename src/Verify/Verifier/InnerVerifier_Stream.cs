partial class InnerVerifier
{
    async Task<VerifyResult> VerifyStream(Stream stream)
    {
        var extension = settings.extension;
        if (extension == null &&
            stream is FileStream fileStream)
        {
            extension = EmptyFiles.Extensions.GetExtension(fileStream.Name);
        }
#if NETSTANDARD2_0 || NETFRAMEWORK || NETCOREAPP2_2 || NETCOREAPP2_1
        using (stream)
#else
        await using (stream)
#endif
        {
            if (stream.Length == 0)
            {
                throw new("Empty data is not allowed.");
            }
            if (extension is not null)
            {
                if (VerifierSettings.HasExtensionConverter(extension))
                {
                    var (infos, convertedTargets, cleanups) = await DoExtensionConversion(extension, stream);
                    object? info = null;
                    if (infos.Count == 1)
                    {
                        info = infos[0];
                    }
                    else if(infos.Count >1)
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
            }

            var targets = await GetTargets(stream, extension);

            return await VerifyInner(null, null, targets);
        }
    }

    async Task<(List<object> infos, List<Target> targets, List<Func<Task>> cleanups)> DoExtensionConversion(string extension, Stream stream)
    {
        var infos = new List<object>();
        var targets = new List<Target>();
        var cleanups = new List<Func<Task>>();

        var queue = new Queue<Target>();
        queue.Enqueue(new(extension,stream));

        while (queue.Count>0)
        {
            var target = queue.Dequeue();

            if (!VerifierSettings.TryGetExtensionConverter(target.Extension, out var conversion))
            {
                targets.Add(target);
                continue;
            }

            var result = await conversion(target.StreamData,settings.Context);
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

        return (infos, targets, cleanups);
    }

    static async Task<List<Target>> GetTargets(Stream stream, string? extension)
    {
        if (extension is not null &&
            EmptyFiles.Extensions.IsText(extension))
        {
            return new()
            {
                new(extension, await stream.ReadString())
            };
        }

        return new()
        {
            new(extension ?? "bin", stream)
        };
    }
}