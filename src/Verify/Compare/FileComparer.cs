static class FileComparer
{
    public static async Task<EqualityResult> DoCompare(VerifySettings settings, FilePair file, bool previousTextFailed, Stream receivedStream)
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

        if (!previousTextFailed &&
            settings.TryFindStreamComparer(file.Extension, out var compare))
        {
            return await InnerCompare(file, receivedStream, (s1, s2) => compare(s1, s2, settings.Context));
        }

        if (receivedStream.CanSeek &&
            FileHelpers.Length(file.VerifiedPath) != receivedStream.Length)
        {
            return new(Equality.NotEqual);
        }

        return await InnerCompare(file, receivedStream, StreamComparer.AreEqual);
    }

    static async Task<EqualityResult> InnerCompare(FilePair file, Stream receivedStream, Func<Stream, Stream, Task<CompareResult>> func)
    {
        using var verifiedStream = FileHelpers.OpenRead(file.VerifiedPath);
        if (receivedStream is FileStream fileStream)
        {
            var compareResult = await func(fileStream, verifiedStream);
            if (compareResult.IsEqual)
            {
                return Equality.Equal;
            }

            File.Copy(fileStream.Name, file.ReceivedPath, true);
            return new(Equality.NotEqual, compareResult.Message);
        }
        else
        {
            using var memoryStream = new MemoryStream();
            await receivedStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var compareResult = await func(memoryStream, verifiedStream);

            if (compareResult.IsEqual)
            {
                return Equality.Equal;
            }

            memoryStream.Position = 0;
            await FileHelpers.WriteStream(file.ReceivedPath, memoryStream);
            return new(Equality.NotEqual, compareResult.Message);
        }
    }
}