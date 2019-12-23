using System.IO;
using System.Threading.Tasks;


static class StreamVerifier
{
    public static async Task<VerifyResult> VerifyStreams(Stream stream, string extension, FilePair file)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        try
        {
            await FileHelpers.WriteStream(file.Received, stream);

            var verifyResult = FileComparer.DoCompare(file.Received, file.Verified, file.Extension);

            if (verifyResult == VerifyResult.Equal)
            {
                File.Delete(file.Received);
                return verifyResult;
            }

            if (!BuildServerDetector.Detected)
            {
                if (DiffTools.TryFindForExtension(extension, out var diffTool))
                {
                    if (EmptyFiles.TryWriteEmptyFile(extension, file.Verified))
                    {
                        DiffRunner.Launch(diffTool, file.Received, file.Verified);
                    }
                }

                await ClipboardCapture.AppendMove(file.Received, file.Verified);
            }

            return verifyResult;
        }
        finally
        {
#if NETSTANDARD2_1
                await stream.DisposeAsync();
#else
            stream.Dispose();
#endif
        }
    }
}