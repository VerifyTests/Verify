static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair filePair, StringBuilder received, VerifySettings settings, bool bypassComparer = false)
    {
        IoHelpers.DeleteFileIfEmpty(filePair.VerifiedPath);
        if (!File.Exists(filePair.VerifiedPath))
        {
            IoHelpers.WriteText(filePair.ReceivedPath, received);
            return new(Equality.New, null, received, null);
        }

        var verifiedText = await File.ReadAllTextAsync(filePair.VerifiedPath);
        if (verifiedText.Contains('\r'))
        {
            throw new($@"Verified file must use \n line endings, but it contains a \r (carriage return). Path: {filePair.VerifiedPath}. See https://github.com/verifytests/verify#text-file-settings");
        }

        var verified = new StringBuilder(verifiedText);
        var result = await CompareStrings(filePair.Extension, received, verified, settings, bypassComparer);
        if (result.IsEqual)
        {
            return new(Equality.Equal, null, received, verified);
        }

        IoHelpers.WriteText(filePair.ReceivedPath, received);
        return new(Equality.NotEqual, result.Message, received, verified);
    }

    static Task<CompareResult> CompareStrings(string extension, StringBuilder received, StringBuilder verified, VerifySettings settings, bool bypassComparer)
    {
        if (verified.Length > 0 &&
            verified.Length - 1 == received.Length &&
            verified.LastChar() == '\n')
        {
            verified.Length -= 1;
        }

        // StringBuilder is broken on older .net https://github.com/dotnet/runtime/issues/27684
#if NET6_0_OR_GREATER
        var isEqual = verified.Equals(received);
        if (!isEqual &&
            !bypassComparer &&
            settings.TryFindStringComparer(extension, out var compare))
        {
            return compare(received.ToString(), verified.ToString(), settings.Context);
        }
#else
        var receivedString = received.ToString();
        var verifiedString = verified.ToString();
        var isEqual = receivedString.Equals(verifiedString);
        if (!isEqual &&
            !bypassComparer &&
            settings.TryFindStringComparer(extension, out var compare))
        {
            return compare(receivedString, verifiedString, settings.Context);
        }
#endif

        return Task.FromResult(new CompareResult(isEqual));
    }
}
