static class StreamExtensions
{
    public static void MoveToStart(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
    }

    public static async Task<string> ReadAsString(this Stream stream)
    {
        stream.MoveToStart();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public static async Task<StringBuilder> ReadAsStringBuilder(this Stream stream)
    {
        var builder = new StringBuilder(await ReadAsString(stream));
        builder.FixNewlines();
        return builder;
    }
}