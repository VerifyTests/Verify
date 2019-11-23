using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        public async Task VerifyText(string target, string extension)
        {
            Guard.AgainstNull(target, nameof(target));
            target = ApplyScrubbers(target);
            var (receivedPath, verifiedPath) = GetFileNames(extension);
            if (!File.Exists(verifiedPath))
            {
                await FileHelpers.WriteText(receivedPath, target);
                ClipboardCapture.Append(receivedPath, verifiedPath);
                if (DiffRunner.FoundDiff)
                {
                    await FileHelpers.WriteText(verifiedPath, "");
                    DiffRunner.Launch(receivedPath, verifiedPath);
                }

                throw new Exception($"First verification. {Context.UniqueTestName}.verified{extension} not found. Verification command has been copied to the clipboard.");
            }

            var verifiedText = await FileHelpers.ReadText(verifiedPath);

            try
            {
                Assert.Equal(verifiedText, target);
            }
            catch (EqualException exception)
            {
                await FileHelpers.WriteText(receivedPath, target);
                ClipboardCapture.Append(receivedPath, verifiedPath);
                exception.PrefixWithCopyCommand();
                DiffRunner.Launch(receivedPath, verifiedPath);

                throw;
            }
        }
    }
}