partial class InnerVerifier
{
    public Task<VerifyResult> VerifyStream(FileStream stream) =>
        VerifyStream(stream, EmptyFiles.Extensions.GetExtension(stream.Name));

    public async Task<VerifyResult> VerifyStream(Task<FileStream> task)
    {
        var stream = await task;
        return await VerifyStream(stream);
    }

    public Task<VerifyResult> VerifyStream(byte[] bytes, string extension) =>
        VerifyStream(new MemoryStream(bytes), extension);

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

    public async Task<VerifyResult> VerifyStreams<T>(IEnumerable<T?> streams, string extension)
        where T : Stream
    {
        var targets = streams.Select(_ =>
        {
            if (_ == null)
            {
                return new("txt", "null");
            }

            return new Target(extension, _);
        });

        return await VerifyInner(null, null, targets);
    }

    public async Task<VerifyResult> VerifyStream(Stream stream, string extension)
    {
        using (stream)
        {
            if (stream.Length == 0)
            {
                throw new("Empty data is not allowed.");
            }

            if (VerifierSettings.TryGetExtensionConverter(extension, out var conversion))
            {
                var result = await conversion(stream, settings.Context);
                return await VerifyInner(result.Info, result.Cleanup, result.Targets);
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
}