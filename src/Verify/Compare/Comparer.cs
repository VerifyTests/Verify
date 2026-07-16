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

        // Read with the configured encoding so a BOM-less non-UTF8 UseEncoding
        // round-trips (writes go through the same encoding).
        var verified = await File.ReadAllTextAsync(filePair.VerifiedPath, VerifierSettings.Encoding);
        if (verified.Contains('\r'))
        {
            // Write received before throwing. Otherwise the run produces no output at all,
            // which reads as "verify silently did nothing" rather than a line ending problem.
            // Accepting the received file also rewrites verified with \n endings.
            IoHelpers.WriteText(filePair.ReceivedPath, received);
            throw new VerifiedLineEndingException(filePair.VerifiedPath);
        }

        var result = await CompareStrings(filePair.Extension, received, verified, settings, bypassComparer);
        if (result.IsEqual)
        {
            return new(Equality.Equal, null, received, verified);
        }

        IoHelpers.WriteText(filePair.ReceivedPath, received);
        return new(Equality.NotEqual, result.Message, received, verified);
    }

    static Task<CompareResult> CompareStrings(string extension, StringBuilder received, string verified, VerifySettings settings, bool bypassComparer)
    {
        var isEqual = received.Equals(verified.AsSpan());
        if (!isEqual &&
            !bypassComparer &&
            settings.TryFindStringComparer(extension, out var compare))
        {
            return compare(received.ToString(), verified, settings.Context);
        }

        return Task.FromResult(new CompareResult(isEqual));
    }
}
