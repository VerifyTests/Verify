using System.IO;
using System.Threading.Tasks;

static class StreamVerifier
{
    public static async Task<VerifyResult> VerifyStreams(Stream stream, string extension, string receivedPath, string verifiedPath)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        try
        {
            await FileHelpers.WriteStream(receivedPath, stream);

            var verifyResult = FileComparer.DoCompare(receivedPath, verifiedPath, extension);

            if (verifyResult == VerifyResult.Equal)
            {
                File.Delete(receivedPath);
                return verifyResult;
            }

            if (!BuildServerDetector.Detected)
            {
                if (DiffTools.TryFindForExtension(extension, out var diffTool))
                {
                    if (EmptyFiles.TryWriteEmptyFile(extension, verifiedPath))
                    {
                        DiffRunner.Launch(diffTool, receivedPath, verifiedPath);
                    }
                }

                await ClipboardCapture.AppendMove(receivedPath, verifiedPath);
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