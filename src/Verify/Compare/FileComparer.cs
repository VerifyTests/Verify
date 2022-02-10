using EmptyFiles;

static class FileComparer
{
    public static async Task<EqualityResult> DoCompare(VerifySettings settings, FilePair file, bool previousTextFailed)
    {
        if (!File.Exists(file.VerifiedPath))
        {
            return Equality.New;
        }

        if (AllFiles.IsEmptyFile(file.VerifiedPath))
        {
            return Equality.NotEqual;
        }

        var result = await FilesEqual(settings, file, previousTextFailed);
        if (result.IsEqual)
        {
            return Equality.Equal;
        }

        return new(Equality.NotEqual, result.Message);
    }

    static Task<CompareResult> FilesEqual(VerifySettings settings, FilePair filePair, bool previousTextFailed)
    {
        if (!previousTextFailed &&
            settings.TryFindStreamComparer(filePair.Extension, out var compare))
        {
            return DoCompare(settings, compare, filePair);
        }

        if (FilesAreSameSize(filePair))
        {
            return DefaultCompare(settings, filePair);
        }

        return Task.FromResult(CompareResult.NotEqual());
    }

    public static Task<CompareResult> DefaultCompare(VerifySettings settings, FilePair filePair)
    {
        return DoCompare(
            settings,
            (stream1, stream2, _) => StreamComparer.AreEqual(stream1, stream2),
            filePair);
    }

    static bool FilesAreSameSize(in FilePair file)
    {
        var first = new FileInfo(file.ReceivedPath);
        var second = new FileInfo(file.VerifiedPath);
        return first.Length == second.Length;
    }

    static async Task<CompareResult> DoCompare(VerifySettings settings, StreamCompare compare, FilePair filePair)
    {
#if NETSTANDARD2_0 || NETFRAMEWORK || NETCOREAPP2_2 || NETCOREAPP2_1
        using var fs1 = FileHelpers.OpenRead(filePair.ReceivedPath);
        using var fs2 = FileHelpers.OpenRead(filePair.VerifiedPath);
#else
        await using var fs1 = FileHelpers.OpenRead(filePair.ReceivedPath);
        await using var fs2 = FileHelpers.OpenRead(filePair.VerifiedPath);
#endif
        return await compare(fs1, fs2, settings.Context);
    }
}