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
            Guard.AgainstNull(input, nameof(input));
            input = ApplyScrubbers(input);
            input = input.Replace("\r\n", "\n");
            var (receivedPath, verifiedPath) = GetFileNames(extension);
            FileHelpers.DeleteIfEmpty(verifiedPath);
            if (!File.Exists(verifiedPath))
            {
                await FileHelpers.WriteText(receivedPath, input);
                ClipboardCapture.Append(receivedPath, verifiedPath);
                if (DiffRunner.FoundDiff)
                {
                    FileHelpers.WriteEmptyText(verifiedPath);
                    DiffRunner.Launch(receivedPath, verifiedPath);
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
            {
                await FileHelpers.WriteText(receivedPath, input);
                ClipboardCapture.Append(receivedPath, verifiedPath);
                exception.PrefixWithCopyCommand();
                DiffRunner.Launch(receivedPath, verifiedPath);

                throw;
            }
        }
    }
}