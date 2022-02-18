partial class InnerVerifier
{
    public Task Verify(byte[] target)
    {
        var stream = new MemoryStream(target);
        return VerifyStream(stream);
    }

    async Task VerifyStream(Stream stream)
    {
        var extension = settings.extension;
#if NETSTANDARD2_0 || NETFRAMEWORK || NETCOREAPP2_2 || NETCOREAPP2_1
        using (stream)
#else
        await using (stream)
#endif
        {
            if (extension is not null)
            {
                if (VerifierSettings.TryGetExtensionConverter(extension, out var conversion))
                {
                    var result = await conversion(stream, settings.Context);
                    await VerifyInner(result.Info, result.Cleanup, result.Targets);
                    return;
                }
            }

            extension ??= "bin";

            List<Target> targets;
            if (EmptyFiles.Extensions.IsText(extension))
            {
                targets = new()
                {
                    new(extension, await stream.ReadString())
                };
                await VerifyInner(null, null, targets);
            }
            else
            {
                targets = new()
                {
                    new(extension, stream)
                };
            }

            await VerifyInner(null, null, targets);
        }
    }
}