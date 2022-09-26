static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair filePair, string receivedText, VerifySettings settings)
    {
        var received = new StringBuilder(receivedText);
        IoHelpers.DeleteFileIfEmpty(filePair.VerifiedPath);
        if (!File.Exists(filePair.VerifiedPath))
        {
            await IoHelpers.WriteText(filePair.ReceivedPath, receivedText);
            return new(Equality.New, null, received, null);
        }

        var verifiedText = await IoHelpers.ReadStringBuilderWithFixedLines(filePair.VerifiedPath);
        var result = await CompareStrings(filePair.Extension, received , verifiedText, settings);
        if (result.IsEqual)
        {
            return new(Equality.Equal, null, received, verifiedText);
        }

        await IoHelpers.WriteText(filePair.ReceivedPath, receivedText);
        return new(Equality.NotEqual, result.Message, received, verifiedText);
    }

    static Task<CompareResult> CompareStrings(string extension, StringBuilder received, StringBuilder verified, VerifySettings settings)
    {
        if (verified.Length > 0 &&
            verified.Length - 1 == received.Length &&
            verified.LastChar() == '\n')
        {
            verified.Length -= 1;
        }

        var verifiedString = verified.ToString();
        var isEqual = verified.Equals(received);


        if (!isEqual &&
            settings.TryFindStringComparer(extension, out var compare))
        {
            return compare(received.ToString(), verifiedString, settings.Context);
        }

        return Task.FromResult(new CompareResult(isEqual));
    }
}