using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace VerifyXunit
{
    public partial class VerifyBase :
        XunitContextBase
    {
        public async Task Verify(
            string? target,
            string extension = ".txt")
        {
            Guard.AgainstNullOrEmpty(extension, nameof(extension));

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

        (string receivedPath, string verifiedPath) GetFileNames(string extension)
        {
            var filePrefix = Path.Combine(SourceDirectory, Context.UniqueTestName);
            var receivedPath = $"{filePrefix}.received{extension}";
            var verifiedPath = $"{filePrefix}.verified{extension}";
            return (receivedPath, verifiedPath);
        }

        public VerifyBase(
            ITestOutputHelper output,
            [CallerFilePath] string sourceFile = "") :
            base(output, sourceFile)
        {
        }
    }
}