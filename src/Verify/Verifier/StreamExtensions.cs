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

    public static async Task<string> ReadAsString(this Stream stream)
    {
        using StreamReader reader = new(stream, FileHelpers.Utf8NoBOM);
        return await reader.ReadToEndAsync();
    }

    public static async Task<StringBuilder> ReadAsStringBuilder(this Stream stream)
    {
        StringBuilder builder = new(await ReadAsString(stream));
        builder.FixNewlines();
        return builder;
    }
}