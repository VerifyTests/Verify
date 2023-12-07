static class FileComparer
{
    public static async Task<EqualityResult> DoCompare(VerifySettings settings, FilePair file, bool previousTextFailed, Stream receivedStream)
    {
        if (!File.Exists(file.VerifiedPath))
        {
            await IoHelpers.WriteStream(file.ReceivedPath, receivedStream);
            return new(Equality.New, null, null, null);
        }

        if (AllFiles.IsEmptyFile(file.VerifiedPath))
        {
            await IoHelpers.WriteStream(file.ReceivedPath, receivedStream);
            return new(Equality.NotEqual, null, null, null);
        }

        if (!previousTextFailed &&
            settings.TryFindStreamComparer(file.Extension, out var compare))
        {
            return await InnerCompare(file, receivedStream, (s1, s2) => compare(s1, s2, settings.Context));
        }

        if (receivedStream.CanSeekAndReadLength() &&
            IoHelpers.Length(file.VerifiedPath) != receivedStream.Length)
        {
            await IoHelpers.WriteStream(file.ReceivedPath, receivedStream);
            return new(Equality.NotEqual, null, null, null);
        }

        return await InnerCompare(file, receivedStream, StreamComparer.AreEqual);
    }

    static async Task<EqualityResult> InnerCompare(FilePair file, Stream receivedStream, Func<Stream, Stream, Task<CompareResult>> func)
    {
#if NETFRAMEWORK
        using var verifiedStream = IoHelpers.OpenRead(file.VerifiedPath);
#else
        await using var verifiedStream = IoHelpers.OpenRead(file.VerifiedPath);
#endif

        if (receivedStream is FileStream fileStream)
        {
            fileStream.ThrowIfEmpty();
            var compareResult = await func(fileStream, verifiedStream);
            if (compareResult.IsEqual)
            {
                return new(Equality.Equal, compareResult.Message, null, null);
            }

            File.Copy(fileStream.Name, file.ReceivedPath, true);
            return new(Equality.NotEqual, compareResult.Message, null, null);
        }

        async Task<EqualityResult> EqualityResult(Stream receivedStream, Stream verifiedStream)
        {
            var compareResult = await func(receivedStream, verifiedStream);

            if (compareResult.IsEqual)
            {
                return new(Equality.Equal, compareResult.Message, null, null);
            }

            receivedStream.MoveToStart();
            receivedStream.ThrowIfEmpty();
            await IoHelpers.WriteStream(file.ReceivedPath, receivedStream);
            return new(Equality.NotEqual, compareResult.Message, null, null);
        }

        if (receivedStream.CanSeekAndReadLength())
        {
            receivedStream.MoveToStart();
            return await EqualityResult(receivedStream, verifiedStream);
        }

        using var memoryStream = new MemoryStream();
        await receivedStream.SafeCopy(memoryStream);
        memoryStream.MoveToStart();

        return await EqualityResult(memoryStream, verifiedStream);
    }
}