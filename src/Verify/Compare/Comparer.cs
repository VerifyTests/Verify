using System.IO;
using System.Text;
using System.Threading.Tasks;
using Verify;

static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair file, StringBuilder received, VerifySettings settings)
    {
        Scrub(received, settings.ignoreTrailingWhitespace);
        FileHelpers.DeleteIfEmpty(file.Verified);
        if (!File.Exists(file.Verified))
        {
            await FileHelpers.WriteText(file.Received, received.ToString());
            return Equality.MissingVerified;
        }

        var verified = await FileHelpers.ReadText(file.Verified);
        Scrub(verified, settings.ignoreTrailingWhitespace);

        var result = await CompareStrings(received, verified, settings);
        if (result.IsEqual)
        {
            return Equality.Equal;
        }

        await FileHelpers.WriteText(file.Received, received.ToString());
        return new EqualityResult(Equality.NotEqual, result.Message);
    }

    static async Task<CompareResult> CompareStrings(StringBuilder received, StringBuilder verified, VerifySettings settings)
    {
        var extension = settings.ExtensionOrTxt();
        if (settings.comparer != null)
        {
            using var stream1 = MemoryStream(received.ToString());
            using var stream2 = MemoryStream(verified.ToString());
            return await settings.comparer(settings, stream1, stream2);
        }
        if (SharedVerifySettings.TryGetComparer(extension, out var comparer))
        {
            using var stream1 = MemoryStream(received.ToString());
            using var stream2 = MemoryStream(verified.ToString());
            return await comparer(settings, stream1, stream2);
        }

        return new CompareResult(verified.Compare(received));
    }

    static MemoryStream MemoryStream(string text)
    {
        return new MemoryStream(FileHelpers.Utf8NoBOM.GetBytes(text));
    }

    static void Scrub(StringBuilder scrubbedInput, bool ignoreTrailingWhitespace)
    {
        scrubbedInput.Replace("\r\n", "\n");
        scrubbedInput.Replace("\r", "\n");
        if (ignoreTrailingWhitespace)
        {
            scrubbedInput.TrimEnd();
        }
    }

    public static async Task<EqualityResult> Streams(
        VerifySettings settings,
        Stream stream,
        FilePair file)
    {
        try
        {
            await FileHelpers.WriteStream(file.Received, stream);

            var result = await FileComparer.DoCompare(settings, file);

            if (result.Equality == Equality.Equal)
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