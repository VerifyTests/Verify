﻿using System.IO;
using System.Threading.Tasks;
using VerifyTests;

static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair filePair, string received, VerifySettings settings)
    {
        FileHelpers.DeleteIfEmpty(filePair.Verified);
        if (!File.Exists(filePair.Verified))
        {
            await FileHelpers.WriteText(filePair.Received, received);
            return Equality.MissingVerified;
        }

        var verified = await FileHelpers.ReadText(filePair.Verified);
        var result = await CompareStrings(received, verified.ToString(), settings);
        if (result.IsEqual)
        {
            return Equality.Equal;
        }

        await FileHelpers.WriteText(filePair.Received, received);
        return new(Equality.NotEqual, result.Message);
    }

    static async Task<CompareResult> CompareStrings(string received, string verified, VerifySettings settings)
    {
        //TODO: implement string comparer
        if (!settings.TryFindComparer(out var compare))
        {
            return new(verified == received);
        }

#if NETSTANDARD2_0 || NETFRAMEWORK
        using var stream1 = MemoryStream(received);
        using var stream2 = MemoryStream(verified);
#else
        await using var stream1 = MemoryStream(received);
        await using var stream2 = MemoryStream(verified);
#endif

        return await compare!(stream1, stream2, settings.Context);
    }

    static MemoryStream MemoryStream(string text)
    {
        return new(FileHelpers.Utf8NoBOM.GetBytes(text));
    }

    public static async Task<EqualityResult> Streams(
        VerifySettings settings,
        Stream stream,
        FilePair file)
    {
        await FileHelpers.WriteStream(file.Received, stream);

        var result = await FileComparer.DoCompare(settings, file);

        if (result.Equality == Equality.Equal)
        {
            File.Delete(file.Received);
        }

        return result;
    }
}