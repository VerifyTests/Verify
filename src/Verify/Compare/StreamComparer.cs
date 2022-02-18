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
            var count = await ReadBufferAsync(stream1, buffer1);

            //no need to compare size here since only enter on files being same size

            if (count == 0)
            {
                return CompareResult.Equal;
            }

            await ReadBufferAsync(stream2, buffer2);

            for (var i = 0; i < count; i += sizeof(long))
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