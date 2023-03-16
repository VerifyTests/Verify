static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair filePair, StringBuilder received, VerifySettings settings)
    {
        IoHelpers.DeleteFileIfEmpty(filePair.VerifiedPath);
        if (!File.Exists(filePair.VerifiedPath))
        {
            await IoHelpers.WriteText(filePair.ReceivedPath, received);
            return new(Equality.New, null, received, null);
        }

        var verified = await IoHelpers.ReadStringBuilderWithFixedLines(filePair.VerifiedPath);
        var result = await CompareStrings(filePair.Extension, received, verified, settings);
        if (result.IsEqual)
        {
            return new(Equality.Equal, null, received, verified);
        }

        await IoHelpers.WriteText(filePair.ReceivedPath, received);
        return new(Equality.NotEqual, result.Message, received, verified);
    }

    static Task<CompareResult> CompareStrings(string extension, StringBuilder received, StringBuilder verified, VerifySettings settings)
    {
        if (verified.Length > 0 &&
            verified.Length - 1 == received.Length &&
            verified.LastChar() == '\n')
        {
            verified.Length -= 1;
        }

        // StringBuilder is broken on older .net https://github.com/dotnet/runtime/issues/27684
        var isEqual = verified.Equals(received);
        if (!isEqual &&
            settings.TryFindStringComparer(extension, out var compare))
        {
            return compare(received.ToString(), verified.ToString(), settings.Context);
        }

        return Task.FromResult(new CompareResult(isEqual));
    }
}