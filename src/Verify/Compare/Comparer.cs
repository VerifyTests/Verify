﻿static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair filePair, string receivedText, VerifySettings settings)
    {
        IoHelpers.DeleteFileIfEmpty(filePair.VerifiedPath);
        if (!File.Exists(filePair.VerifiedPath))
        {
            await IoHelpers.WriteText(filePair.ReceivedPath, receivedText);
            return new(Equality.New, null, receivedText, null);
        }

        var verifiedText = await IoHelpers.ReadStringBuilderWithFixedLines(filePair.VerifiedPath);
        var result = await CompareStrings(filePair.Extension, receivedText, verifiedText, settings);
        if (result.IsEqual)
        {
            return new(Equality.Equal, null, receivedText, verifiedText);
        }

        await IoHelpers.WriteText(filePair.ReceivedPath, receivedText);
        return new(Equality.NotEqual, result.Message, receivedText, verifiedText);
    }

    static Task<CompareResult> CompareStrings(string extension, string received, StringBuilder verified, VerifySettings settings)
    {
        if (verified.Length > 0 &&
            verified.Length - 1 == received.Length &&
            verified.LastChar() == '\n')
        {
            verified.Length -= 1;
        }

        var verifiedString = verified.ToString();
        var isEqual = string.Equals(verifiedString, received, StringComparison.Ordinal);


        if (!isEqual &&
            settings.TryFindStringComparer(extension, out var compare))
        {
            return compare(received, verifiedString, settings.Context);
        }

        return Task.FromResult(new CompareResult(isEqual));
    }
}