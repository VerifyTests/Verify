static class StreamComparer
{
    #region DefualtCompare

    const int bufferSize = 1024 * sizeof(long);

    public static async Task<CompareResult> AreEqual(Stream stream1, Stream stream2)
    {
        EnsureAtStart(stream1);
        EnsureAtStart(stream2);

        var buffer1 = ArrayPool<byte>.Shared.Rent(bufferSize);
        var buffer2 = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            while (true)
            {
                var count1 = await ReadBufferAsync(stream1, buffer1);
                var count2 = await ReadBufferAsync(stream2, buffer2);

                // Callers do not always guarantee the streams are the same length
                // (e.g. a non-seekable received stream), so a length difference must
                // be treated as not-equal instead of a short-circuit to equal.
                if (count1 != count2)
                {
                    return CompareResult.NotEqual();
                }

                if (count1 == 0)
                {
                    return CompareResult.Equal;
                }

                // Compare exactly the bytes read (a rented buffer's trailing bytes
                // are arbitrary, so they must not be included).
                if (!buffer1.AsSpan(0, count1).SequenceEqual(buffer2.AsSpan(0, count1)))
                {
                    return CompareResult.NotEqual();
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer1);
            ArrayPool<byte>.Shared.Return(buffer2);
        }
    }

    static void EnsureAtStart(Stream stream)
    {
        if (stream.CanSeek &&
            stream.Position != 0)
        {
            throw new("Expected stream to be at position 0.");
        }
    }

    static async Task<int> ReadBufferAsync(Stream stream, byte[] buffer)
    {
        // Read up to bufferSize (not buffer.Length): a rented buffer may be
        // larger, and both streams must be read in equal-sized chunks so the
        // length comparison above stays aligned.
        var bytesRead = 0;
        while (bytesRead < bufferSize)
        {
            var read = await stream.ReadAsync(buffer, bytesRead, bufferSize - bytesRead);
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