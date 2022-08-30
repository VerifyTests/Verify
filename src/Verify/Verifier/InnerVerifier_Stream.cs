partial class InnerVerifier
{
    public Task<VerifyResult> Verify(byte[] target)
    {
        var stream = new MemoryStream(target);
        return VerifyStream(stream);
    }

    async Task<VerifyResult> VerifyStream(Stream stream)
    {
        var extension = settings.extension;
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
                if (VerifierSettings.TryGetExtensionConverter(extension, out var conversion))
                {
                    var result = await conversion(stream, settings.Context);
                    return await VerifyInner(result.Info, result.Cleanup, result.Targets);
                }
            }

            var targets = await GetTargets(stream, extension);

            return await VerifyInner(null, null, targets);
        }
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