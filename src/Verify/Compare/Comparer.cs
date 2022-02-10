static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair filePair, string received, VerifySettings settings)
    {
        FileHelpers.DeleteIfEmpty(filePair.VerifiedPath);
        if (!File.Exists(filePair.VerifiedPath))
        {
            await FileHelpers.WriteText(filePair.ReceivedPath, received);
            return Equality.New;
        }

        var verified = await FileHelpers.ReadText(filePair.VerifiedPath);
        var result = await CompareStrings(filePair.Extension, received, verified.ToString(), settings);
        if (result.IsEqual)
        {
            return Equality.Equal;
        }

        await FileHelpers.WriteText(filePair.ReceivedPath, received);
        return new(Equality.NotEqual, result.Message);
    }

    static Task<CompareResult> CompareStrings(string extension, string received, string verified, VerifySettings settings)
    {
        var isEqual = verified == received;

        if (!isEqual &&
            settings.TryFindStringComparer(extension, out var compare))
        {
            return compare(received, verified, settings.Context);
        }

        return Task.FromResult(new CompareResult(isEqual));
    }
}