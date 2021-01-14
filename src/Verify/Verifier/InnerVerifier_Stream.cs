using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    partial class InnerVerifier
    {
        public Task Verify(byte[] target)
        {
            Guard.AgainstNull(target, nameof(target));
            MemoryStream stream = new(target);
            return VerifyStream(stream);
        }

        async Task VerifyStream(Stream stream)
        {
            var extension = settings.extension;
#if NETSTANDARD2_0 || NETFRAMEWORK
            using (stream)
#else
            await using (stream)
#endif
            {
                if (extension != null)
                {
                    if (VerifierSettings.TryGetExtensionConverter(extension, out var conversion))
                    {
                        var result = await conversion(stream, settings.Context);
                        await VerifyInner(result.Info, result.Cleanup, result.Streams);
                        return;
                    }
                }

                extension ??= "bin";

                List<ConversionStream> streams = new()
                {
                    new(extension, stream)
                };
                await VerifyInner(null, null, streams);
            }
        }
    }
}