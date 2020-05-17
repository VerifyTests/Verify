using System;
using System.IO;
using System.Threading.Tasks;
using EmptyFiles;
using Verify;

static class FileComparer
{
    public static async Task<EqualityResult> DoCompare(VerifySettings settings, FilePair file)
    {
        if (!File.Exists(file.Verified))
        {
            return Equality.MissingVerified;
        }

        if (AllFiles.IsEmptyFile(file.Verified))
        {
            return Equality.NotEqual;
        }

        var compareResult = await FilesEqual(settings, file);
        if (compareResult.IsEqual)
        {
            return Equality.Equal;
        }

        return new EqualityResult(Equality.NotEqual, compareResult.Message);
    }

    public static Task<CompareResult> FilesEqual(VerifySettings settings, FilePair file)
    {
        if (settings.TryFindComparer(out var compare))
        {
            return DoCompare(settings, file.Received, file.Verified, compare);
        }

        if (!FilesAreSameSize(file))
        {
            return Task.FromResult(CompareResult.NotEqual());
        }

        return DefaultCompare(settings, file.Received, file.Verified);
    }

    public static Task<CompareResult> DefaultCompare(VerifySettings settings, string received, string verified)
    {
        return DoCompare(
            settings,
            received,
            verified,
            (verifySettings, stream1, stream2) => StreamsAreEqual(stream1, stream2));
    }

    static bool FilesAreSameSize(FilePair file)
    {
        var first = new FileInfo(file.Received);
        var second = new FileInfo(file.Verified);
        return first.Length == second.Length;
    }

    static async Task<CompareResult> DoCompare(VerifySettings settings, string first, string second, Compare compare)
    {
#if NETSTANDARD2_1
        await using var fs1 = FileHelpers.OpenRead(first);
        await using var fs2 = FileHelpers.OpenRead(second);
#else
        using var fs1 = FileHelpers.OpenRead(first);
        using var fs2 = FileHelpers.OpenRead(second);
#endif
        return await compare(settings, fs1, fs2);
    }

    #region DefualtCompare
    static async Task<CompareResult> StreamsAreEqual(Stream stream1, Stream stream2)
    {
        const int bufferSize = 1024 * sizeof(long);
        var buffer1 = new byte[bufferSize];
        var buffer2 = new byte[bufferSize];

        while (true)
        {
            var t1 = ReadBufferAsync(stream1, buffer1);
            await ReadBufferAsync(stream2, buffer2);

            var count = await t1;

            //no need to compare size here since only enter on files being same size

            if (count == 0)
            {
                return CompareResult.Equal;
            }

            for (var i = 0; i < count; i+= sizeof(long))
            {
                if (BitConverter.ToInt64(buffer1, i) != BitConverter.ToInt64(buffer2, i))
                {
                    return CompareResult.NotEqual();
                }
            }
        }
    }

    static async Task<int> ReadBufferAsync(Stream stream, byte[] buffer)
    {
        var bytesRead = 0;
        while (bytesRead < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer, bytesRead, buffer.Length - bytesRead);
            if (read == 0)
            {
                // Reached end of stream.
                return bytesRead;
            }

            bytesRead += read;
        }

        return bytesRead;
    }
    #endregion
}