using System.IO;
using System.Threading.Tasks;

static class StreamVerifier
{
    public static async Task<VerifyResult> VerifyStreams(Stream stream, FilePair file)
    {
        stream.MoveToStart();

        try
        {
            await FileHelpers.WriteStream(file.Received, stream);

            var verifyResult = FileComparer.DoCompare(file.Received, file.Verified);

            if (verifyResult == VerifyResult.Equal)
            {
                File.Delete(file.Received);
                return verifyResult;
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