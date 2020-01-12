using System;
using System.IO;
using System.Threading.Tasks;

static class Comparer
{
    public static async Task<CompareResult> Text(FilePair file, string scrubbedInput)
    {
        FileHelpers.DeleteIfEmpty(file.Verified);
        if (!File.Exists(file.Verified))
        {
            await FileHelpers.WriteText(file.Received, scrubbedInput);
            return CompareResult.MissingVerified;
        }

        var verifiedText = await FileHelpers.ReadText(file.Verified);
        verifiedText = verifiedText.Replace("\r\n", "\n");
        if (!string.Equals(verifiedText, scrubbedInput, StringComparison.OrdinalIgnoreCase))
        {
            await FileHelpers.WriteText(file.Received, scrubbedInput);
            return CompareResult.NotEqual;
        }

        return CompareResult.Equal;
    }

    public static async Task<CompareResult> Streams(Stream stream, FilePair file)
    {
        stream.MoveToStart();

        try
        {
            await FileHelpers.WriteStream(file.Received, stream);

            var result = FileComparer.DoCompare(file.Received, file.Verified);

            if (result == CompareResult.Equal)
            {
                File.Delete(file.Received);
                return result;
            }

            return result;
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