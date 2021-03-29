using System.IO;
using System.Threading.Tasks;
using VerifyTests;

static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair filePair, string received, VerifySettings settings)
    {
        FileHelpers.DeleteIfEmpty(filePair.Verified);
        if (!File.Exists(filePair.Verified))
        {
            await FileHelpers.WriteText(filePair.Received, received);
            return Equality.MissingVerified;
        }

        var verified = await FileHelpers.ReadText(filePair.Verified);
        var result = await CompareStrings(filePair.Extension, received, verified.ToString(), settings);
        if (result.IsEqual)
        {
            return Equality.Equal;
        }

        await FileHelpers.WriteText(filePair.Received, received);
        return new(Equality.NotEqual, result.Message);
    }

    static Task<CompareResult> CompareStrings(string extension, string received, string verified, VerifySettings settings)
    {
        var isEqual = verified == received;

        if (!isEqual &&
            settings.TryFindStringComparer(extension, out var compare))
        {
            return compare!(received, verified, settings.Context);
        }

        return Task.FromResult(new CompareResult(isEqual));
    }

    public static async Task<EqualityResult> Streams(
        VerifySettings settings,
        Stream stream,
        FilePair file,
        bool previousTextHasFailed)
    {
        await FileHelpers.WriteStream(file.Received, stream);

        var result = await FileComparer.DoCompare(settings, file, previousTextHasFailed);

        if (result.Equality == Equality.Equal)
        {
            File.Delete(file.Received);
        }

        return result;
    }
}