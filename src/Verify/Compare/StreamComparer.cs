static class StreamComparer
{
    #region DefualtCompare

    const int bufferSize = 1024 * sizeof(long);

    public static async Task<CompareResult> AreEqual(Stream stream1, Stream stream2)
    {
        EnsureAtStart(stream1);
        EnsureAtStart(stream2);

        var buffer1 = new byte[bufferSize];
        var buffer2 = new byte[bufferSize];

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

            for (var i = 0; i < count1; i += sizeof(long))
            {
                if (BitConverter.ToInt64(buffer1, i) != BitConverter.ToInt64(buffer2, i))
                {
                    return CompareResult.NotEqual();
                }
            }
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