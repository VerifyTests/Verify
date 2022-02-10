using EmptyFiles;

static class FileComparer
{
    public static async Task<EqualityResult> DoCompare(VerifySettings settings, FilePair file, bool previousTextHasFailed, Stream receivedStream)
    {
        if (!File.Exists(file.VerifiedPath))
        {
            await FileHelpers.WriteStream(file.ReceivedPath, receivedStream);
            return Equality.New;
        }

        if (AllFiles.IsEmptyFile(file.VerifiedPath))
        {
            await FileHelpers.WriteStream(file.ReceivedPath, receivedStream);
            return Equality.NotEqual;
        }

        var result = await FilesEqual(settings, file, previousTextHasFailed);
        if (!result.IsEqual)
        {
            return new(Equality.NotEqual, result.Message);
        }

        return Equality.Equal;

    }

    static Task<CompareResult> FilesEqual(VerifySettings settings, FilePair filePair, bool previousTextHasFailed)
    {
        if (!previousTextHasFailed &&
            settings.TryFindStreamComparer(filePair.Extension, out var compare))
        {
            return DoCompare(settings, compare, filePair);
        }

        if (FilesAreSameSize(filePair))
        {
            return DoCompare(settings, (stream1, stream2, _) => StreamsAreEqual(stream1, stream2), filePair);
        }

        return Task.FromResult(CompareResult.NotEqual());
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