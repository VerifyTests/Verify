using VerifyTests;

partial class InnerVerifier
{
    public Task Verify(byte[] target)
    {
        MemoryStream stream = new(target);
        return VerifyStream(stream);
    }

    async Task VerifyStream(Stream stream)
    {
        var extension = settings.extension;
        using (stream)
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
                    new(extension, await stream.ReadAsString())
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
