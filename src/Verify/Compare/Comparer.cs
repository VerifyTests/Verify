static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair filePair, string receivedText, VerifySettings settings)
    {
        IoHelpers.DeleteFileIfEmpty(filePair.VerifiedPath);
        if (!File.Exists(filePair.VerifiedPath))
        {
            await IoHelpers.WriteText(filePair.ReceivedPath, receivedText);
            return new(Equality.New, null, receivedText, null);
        }

        var verifiedText = await IoHelpers.ReadStringWithFixedLines(filePair.VerifiedPath);
        var result = await CompareStrings(filePair.Extension, receivedText, verifiedText, settings);
        if (result.IsEqual)
        {
            return new(Equality.Equal, null, receivedText, verifiedText);
        }

        await IoHelpers.WriteText(filePair.ReceivedPath, receivedText);
        return new(Equality.NotEqual, result.Message, receivedText, verifiedText);
    }

    static Task<CompareResult> CompareStrings(string extension, string received, string verified, VerifySettings settings)
    {
        if (verified.Length > 0 &&
            verified.Length - 1 == received.Length &&
            verified[^1] == '\n')
        {
            verified = verified[..^1];
        }

        var isEqual = string.Equals(verified, received, StringComparison.Ordinal);


        if (!isEqual &&
            settings.TryFindStringComparer(extension, out var compare))
        {
            return compare(received, verified, settings.Context);
        }

        return Task.FromResult(new CompareResult(isEqual));
    }
}