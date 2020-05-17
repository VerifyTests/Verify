﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using Verify;

static class Comparer
{
    public static async Task<EqualityResult> Text(FilePair file, StringBuilder received, VerifySettings settings)
    {
        FileHelpers.DeleteIfEmpty(file.Verified);
        if (!File.Exists(file.Verified))
        {
            await FileHelpers.WriteText(file.Received, received.ToString());
            return Equality.MissingVerified;
        }

        var verified = await FileHelpers.ReadText(file.Verified);

        var result = await CompareStrings(received, verified, settings);
        if (result.IsEqual)
        {
            return Equality.Equal;
        }

        await FileHelpers.WriteText(file.Received, received.ToString());
        return new EqualityResult(Equality.NotEqual, result.Message);
    }

    static async Task<CompareResult> CompareStrings(StringBuilder received, StringBuilder verified, VerifySettings settings)
    {
        if (!settings.TryFindComparer(out var compare))
        {
            return new CompareResult(verified.Compare(received));
        }

        var receivedText = received.ToString();
        var verifiedText = verified.ToString();
#if NETSTANDARD2_1
        await using var stream1 = MemoryStream(receivedText);
        await using var stream2 = MemoryStream(verifiedText);
#else
        using var stream1 = MemoryStream(receivedText);
        using var stream2 = MemoryStream(verifiedText);
#endif

        return await compare(settings, stream1, stream2);
    }

    static MemoryStream MemoryStream(string text)
    {
        return new MemoryStream(FileHelpers.Utf8NoBOM.GetBytes(text));
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