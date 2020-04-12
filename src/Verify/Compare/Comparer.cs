using System;
using System.IO;
using System.Threading.Tasks;
using Verify;

static class Comparer
{
    public static async Task<CompareResult> Text(FilePair file, string scrubbedInput, bool ignoreTrailingWhitespace)
    {
        scrubbedInput = Scrub(scrubbedInput, ignoreTrailingWhitespace);
        FileHelpers.DeleteIfEmpty(file.Verified);
        if (!File.Exists(file.Verified))
        {
            await FileHelpers.WriteText(file.Received, scrubbedInput);
            return CompareResult.MissingVerified;
        }

        var verifiedText = await FileHelpers.ReadText(file.Verified);
        verifiedText = Scrub(verifiedText, ignoreTrailingWhitespace);
        if (string.Equals(verifiedText, scrubbedInput, StringComparison.OrdinalIgnoreCase))
        {
            return CompareResult.Equal;
        }

        await FileHelpers.WriteText(file.Received, scrubbedInput);
        return CompareResult.NotEqual;
    }

    static string Scrub(string scrubbedInput, bool ignoreTrailingWhitespace)
    {
        scrubbedInput = scrubbedInput.Replace("\r\n", "\n");
        if (ignoreTrailingWhitespace)
        {
            scrubbedInput = scrubbedInput.TrimEnd();
        }

        return scrubbedInput;
    }

    public static async Task<CompareResult> Streams(VerifySettings settings, Stream stream, FilePair file)
    {
        try
        {
            await FileHelpers.WriteStream(file.Received, stream);

            var result = await FileComparer.DoCompare(settings, file);

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