static class FileComparer
{
    public static async Task<EqualityResult> DoCompare(VerifySettings settings, FilePair file, bool bypassComparer, Stream receivedStream)
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

        if (!bypassComparer &&
            settings.TryFindStreamComparer(file.Extension, out var compare))
        {
            return await InnerCompare(file, receivedStream, compare, settings.Context);
        }

        if (receivedStream.CanSeekAndReadLength() &&
            IoHelpers.Length(file.VerifiedPath) != receivedStream.Length)
        {
            await IoHelpers.WriteStream(file.ReceivedPath, receivedStream);
            return new(Equality.NotEqual, null, null, null);
        }

        return await InnerCompare(file, receivedStream, static (s1, s2, _) => StreamComparer.AreEqual(s1, s2), null!);
    }

    static async Task<EqualityResult> InnerCompare(FilePair file, Stream receivedStream, StreamCompare compare, IReadOnlyDictionary<string, object> context)
    {
#if NETFRAMEWORK
        using var verifiedStream = IoHelpers.OpenRead(file.VerifiedPath);
#else
        await using var verifiedStream = IoHelpers.OpenRead(file.VerifiedPath);
#endif

        if (receivedStream is FileStream fileStream)
        {
            fileStream.ThrowIfEmpty();
            var compareResult = await compare(fileStream, verifiedStream, context);
            if (compareResult.IsEqual)
            {
                return new(Equality.Equal, compareResult.Message, null, null);
            }

            // Not CopyFile(fileStream.Name): a FileStream built from a handle has no
            // usable Name. WriteStream keeps the copy-by-path fast path and falls back
            // to the handle, which is what the New and empty paths above already do.
            await IoHelpers.WriteStream(file.ReceivedPath, fileStream);
            return new(Equality.NotEqual, compareResult.Message, null, null);
        }

        async Task<EqualityResult> EqualityResult(Stream receivedStream, Stream verifiedStream)
        {
            var compareResult = await compare(receivedStream, verifiedStream, context);

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