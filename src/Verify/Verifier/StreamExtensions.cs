﻿using System.IO;
using System.Text;
using System.Threading.Tasks;

static class StreamExtensions
{
    public static void MoveToStart(this Stream stream)
    {
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }
    }

    public static async Task<StringBuilder> ReadAsString(this Stream stream)
    {
        using var reader = new StreamReader(stream, FileHelpers.Utf8NoBOM);
        var builder = new StringBuilder((int) stream.Length);

        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            builder.Append(line);
            builder.Append('\n');
        }

        if (builder.Length > stream.Length)
        {
            builder.Length -= 1;
        }

        return builder;
    }
}