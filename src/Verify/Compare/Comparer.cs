static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair filePair, string receivedText, VerifySettings settings)
    {
        IoHelpers.DeleteIfEmpty(filePair.VerifiedPath);
        if (!File.Exists(filePair.VerifiedPath))
        {
            await IoHelpers.WriteText(filePair.ReceivedPath, receivedText);
            return new EqualityResult(Equality.New, null, receivedText, null);
        }

        var verifiedText = await IoHelpers.ReadStringWithFixedLines(filePair.VerifiedPath);
        var result = await CompareStrings(filePair.Extension, receivedText, verifiedText, settings);
        if (result.IsEqual)
        {
            return new EqualityResult(Equality.Equal, null, receivedText, verifiedText);
        }

        await IoHelpers.WriteText(filePair.ReceivedPath, receivedText);
        return new(Equality.NotEqual, result.Message, receivedText, verifiedText);
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