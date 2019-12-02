using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public async Task Verify(string input, string extension)
        {
            Guard.AgainstBadExtension(extension, nameof(extension));
            Guard.AgainstNull(input, nameof(input));
            input = ApplyScrubbers(input);
            input = input.Replace("\r\n", "\n");
            var (receivedPath, verifiedPath) = GetFileNames(extension);
            FileHelpers.DeleteIfEmpty(verifiedPath);
            if (!File.Exists(verifiedPath))
            {
                if (!BuildServerDetector.Detected)
                {
                    await FileHelpers.WriteText(receivedPath, input);
                    await ClipboardCapture.Append(receivedPath, verifiedPath);
                    if (DiffTools.TryGetTextDiff(extension, out var diffTool))
                    {
                        FileHelpers.WriteEmptyText(verifiedPath);
                        DiffRunner.Launch(diffTool, receivedPath, verifiedPath);
                    }
                }

                throw VerificationNotFoundException(extension);
            }

            var verifiedText = await FileHelpers.ReadText(verifiedPath);
            verifiedText = verifiedText.Replace("\r\n", "\n");
            try
            {
                Assert.Equal(verifiedText, input);
            }
            catch (EqualException exception)
                when (!BuildServerDetector.Detected)
            {
                exception.PrefixWithCopyCommand();
                await FileHelpers.WriteText(receivedPath, input);
                await ClipboardCapture.Append(receivedPath, verifiedPath);
                if (DiffTools.TryGetTextDiff(extension, out var diffTool))
                {
                    DiffRunner.Launch(diffTool, receivedPath, verifiedPath);
                }

                throw;
            }
        }
    }
}