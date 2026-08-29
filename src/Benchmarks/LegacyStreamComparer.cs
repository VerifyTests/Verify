using System.Buffers;

// StreamComparer as it stood before the current branch, with the two things the branch
// changed inside ReadBufferAsync lifted to parameters, so each step can be measured on
// its own:
//  * bufferSize: 1024 * sizeof(long) (8K) was the old constant, 64K is the new one
//  * useMemoryOverload: the byte[] ReadAsync overload wraps every call in a Task on a
//    FileStream opened for async IO, the Memory<byte> overload does not
// The reads still run one after the other, which is the part the branch replaced with
// an overlapped pair.
static class LegacyStreamComparer
{
    public static async Task<CompareResult> AreEqual(Stream stream1, Stream stream2, int bufferSize, bool useMemoryOverload)
    {
        var buffer1 = ArrayPool<byte>.Shared.Rent(bufferSize);
        var buffer2 = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            while (true)
            {
                var count1 = await ReadBufferAsync(stream1, buffer1, bufferSize, useMemoryOverload);
                var count2 = await ReadBufferAsync(stream2, buffer2, bufferSize, useMemoryOverload);

                if (count1 != count2)
                {
                    return CompareResult.NotEqual();
                }

                if (count1 == 0)
                {
                    return CompareResult.Equal;
                }

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

    static async Task<int> ReadBufferAsync(Stream stream, byte[] buffer, int bufferSize, bool useMemoryOverload)
    {
        var bytesRead = 0;
        while (bytesRead < bufferSize)
        {
            int read;
            if (useMemoryOverload)
            {
                read = await stream.ReadAsync(buffer.AsMemory(bytesRead, bufferSize - bytesRead));
            }
            else
            {
                read = await stream.ReadAsync(buffer, bytesRead, bufferSize - bytesRead);
            }

            if (read == 0)
            {
                // Reached end of stream.
                return bytesRead;
            }

            bytesRead += read;
        }

        return bytesRead;
    }
}
