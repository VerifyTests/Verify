using System;
using System.IO;
using System.Threading.Tasks;

namespace VerifyXunit
{
    public partial class VerifyBase
    {
        public Task Verify(Stream input)
        {
            return Verify(input, ".bin");
        }

        public async Task Verify(Stream input, string extension)
        {
            Guard.AgainstNull(input, nameof(input));

            try
            {
                var (receivedPath, verifiedPath) = GetFileNames(extension);
                FileHelpers.DeleteIfEmpty(verifiedPath);
                await FileHelpers.WriteStream(receivedPath, input);
                if (!File.Exists(verifiedPath))
                {
                    ClipboardCapture.Append(receivedPath, verifiedPath);
                    if (DiffRunner.FoundDiff)
                    {
                        FileHelpers.WriteEmpty(verifiedPath);
                        DiffRunner.Launch(receivedPath, verifiedPath);
                    }

                    throw new Exception($"First verification. {Context.UniqueTestName}.verified{extension} not found. Verification command has been copied to the clipboard.");
                }

                if (FileHelpers.FilesEqual(receivedPath, verifiedPath))
                {
                    return;
                }

                ClipboardCapture.Append(receivedPath, verifiedPath);
                throw new Exception("Streams not equal.");
            }
            finally
            {
                input.Dispose();
            }
        }
    }
}