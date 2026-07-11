// Copies of the pre-engine StringBuilder based scrubber implementations, preserved so the
// benchmarks can report before/after rows over identical corpora. Sourced from
// src/Verify/Serialization/Scrubbers (deleted when the span engine replaced them).

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

static class LegacyGuidScrubber
{
    public static void ReplaceGuids(StringBuilder builder, Counter counter)
    {
        if (!counter.ScrubGuids)
        {
            return;
        }

        //{173535ae-995b-4cc6-a74e-8cd4be57039c}
        if (builder.Length < 36)
        {
            return;
        }

        var matches = FindMatches(builder, counter);

        // Sort by position descending. In-place to avoid LINQ allocation
        matches.Sort((a, b) => b.Index.CompareTo(a.Index));

        // Apply matches
        foreach (var match in matches)
        {
            builder.Overwrite(match.Value, match.Index, 36);
        }
    }

    static List<Match> FindMatches(StringBuilder builder, Counter counter)
    {
        var absolutePosition = 0;
        var matches = new List<Match>();
        Span<char> carryoverBuffer = stackalloc char[35];
        Span<char> buffer = stackalloc char[36];
        var carryoverLength = 0;
        var previousChunkAbsoluteEnd = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var chunkSpan = chunk.Span;

            // Check for GUIDs spanning from previous chunk to current chunk
            if (carryoverLength > 0)
            {
                // Check each possible starting position in the carryover
                for (var carryoverIndex = 0; carryoverIndex < carryoverLength; carryoverIndex++)
                {
                    var remainingInCarryover = carryoverLength - carryoverIndex;
                    var neededFromCurrent = 36 - remainingInCarryover;

                    if (neededFromCurrent <= 0 ||
                        chunkSpan.Length < neededFromCurrent)
                    {
                        continue;
                    }

                    carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(buffer);
                    chunkSpan[..neededFromCurrent].CopyTo(buffer[remainingInCarryover..]);

                    // Check boundary characters
                    var startPosition = previousChunkAbsoluteEnd - carryoverLength + carryoverIndex;

                    var hasValidStart = startPosition == 0 ||
                                        !IsInvalidStartingChar(builder[startPosition - 1]);

                    if (!hasValidStart)
                    {
                        continue;
                    }

                    var hasValidEnd = neededFromCurrent >= chunkSpan.Length ||
                                      !IsInvalidEndingChar(chunkSpan[neededFromCurrent]);

                    if (!hasValidEnd)
                    {
                        continue;
                    }

                    if (!Guid.TryParseExact(buffer, "D", out var guid))
                    {
                        continue;
                    }

                    var convert = counter.Convert(guid);
                    matches.Add(new(startPosition, convert));
                }
            }

            // Process GUIDs entirely within this chunk
            if (chunk.Length >= 36)
            {
                for (var chunkIndex = 0; chunkIndex < chunk.Length; chunkIndex++)
                {
                    var end = chunkIndex + 36;
                    if (end > chunk.Length)
                    {
                        break;
                    }

                    var value = chunkSpan;
                    if ((chunkIndex != 0 && IsInvalidStartingChar(value[chunkIndex - 1])) ||
                        (end != value.Length && IsInvalidEndingChar(value[end])))
                    {
                        continue;
                    }

                    var slice = value.Slice(chunkIndex, 36);

                    if (!Guid.TryParseExact(slice, "D", out var guid))
                    {
                        continue;
                    }

                    var convert = counter.Convert(guid);
                    var startReplaceIndex = absolutePosition + chunkIndex;
                    matches.Add(new(startReplaceIndex, convert));
                    chunkIndex += 35;
                }
            }

            // Roll the carryover forward: keep the last 35 chars of everything seen so far
            if (chunk.Length >= 35)
            {
                chunkSpan.Slice(chunk.Length - 35, 35).CopyTo(carryoverBuffer);
                carryoverLength = 35;
            }
            else
            {
                var keep = Math.Min(carryoverLength, 35 - chunk.Length);
                carryoverBuffer.Slice(carryoverLength - keep, keep).CopyTo(carryoverBuffer);
                chunkSpan.CopyTo(carryoverBuffer[keep..]);
                carryoverLength = keep + chunk.Length;
            }

            previousChunkAbsoluteEnd = absolutePosition + chunk.Length;
            absolutePosition += chunk.Length;
        }

        return matches;
    }

    static bool IsInvalidEndingChar(char ch) =>
        IsInvalidChar(ch) &&
        ch != '}' &&
        ch != ')';

    static bool IsInvalidChar(char ch) =>
        char.IsLetter(ch) ||
        char.IsNumber(ch);

    static bool IsInvalidStartingChar(char ch) =>
        IsInvalidChar(ch) &&
        ch != '{' &&
        ch != '(';

    readonly struct Match(int index, string value)
    {
        public readonly int Index = index;
        public readonly string Value = value;
    }
}

static class LegacyDateScrubber
{
    delegate bool TryConvert(
        ReadOnlySpan<char> span,
        string format,
        Counter counter,
        CultureInfo culture,
        [NotNullWhen(true)] out string? result);

    static bool TryConvertDate(
        ReadOnlySpan<char> span,
        string format,
        Counter counter,
        CultureInfo culture,
        [NotNullWhen(true)] out string? result)
    {
        if (DateOnly.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
        {
            result = counter.Convert(date);
            return true;
        }

        result = null;
        return false;
    }

    public static Action<StringBuilder, Counter> BuildDateScrubber(string format, CultureInfo? culture) =>
        (builder, counter) => ReplaceDates(builder, format, counter, culture ?? CultureInfo.CurrentCulture);

    public static void ReplaceDates(StringBuilder builder, string format, Counter counter, CultureInfo culture) =>
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            TryConvertDate);

    public static Action<StringBuilder, Counter> BuildDateTimeOffsetScrubber(string format, CultureInfo? culture) =>
        (builder, counter) => ReplaceDateTimeOffsets(builder, format, counter, culture ?? CultureInfo.CurrentCulture);

    static bool TryConvertDateTimeOffset(
        ReadOnlySpan<char> span,
        string format,
        Counter counter,
        CultureInfo culture,
        [NotNullWhen(true)] out string? result)
    {
        if (DateTimeOffset.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
        {
            result = counter.Convert(date);
            return true;
        }

        result = null;
        return false;
    }

    public static void ReplaceDateTimeOffsets(StringBuilder builder, string format, Counter counter, CultureInfo culture)
    {
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            TryConvertDateTimeOffset);
        if (TryGetFormatWithUpperMillisecondsTrimmed(format, out var trimmedFormat))
        {
            ReplaceInner(
                builder,
                trimmedFormat,
                counter,
                culture,
                TryConvertDateTimeOffset);
        }
    }

    static bool TryConvertDateTime(
        ReadOnlySpan<char> span,
        string format,
        Counter counter,
        CultureInfo culture,
        [NotNullWhen(true)] out string? result)
    {
        if (DateTime.TryParseExact(span, format, culture, DateTimeStyles.None, out var date))
        {
            result = counter.Convert(date);
            return true;
        }

        result = null;
        return false;
    }

    public static Action<StringBuilder, Counter> BuildDateTimeScrubber(string format, CultureInfo? culture) =>
        (builder, counter) => ReplaceDateTimes(builder, format, counter, culture ?? CultureInfo.CurrentCulture);

    public static void ReplaceDateTimes(StringBuilder builder, string format, Counter counter, CultureInfo culture)
    {
        ReplaceInner(
            builder,
            format,
            counter,
            culture,
            TryConvertDateTime);
        if (TryGetFormatWithUpperMillisecondsTrimmed(format, out var trimmedFormat))
        {
            ReplaceInner(
                builder,
                trimmedFormat,
                counter,
                culture,
                TryConvertDateTime);
        }
    }

    static bool TryGetFormatWithUpperMillisecondsTrimmed(string format, [NotNullWhen(true)] out string? trimmedFormat)
    {
        if (format.EndsWith(".FFFF"))
        {
            trimmedFormat = format[..^5];
            return true;
        }

        if (format.EndsWith(".FFF"))
        {
            trimmedFormat = format[..^4];
            return true;
        }

        if (format.EndsWith(".FF"))
        {
            trimmedFormat = format[..^3];
            return true;
        }

        if (format.EndsWith(".F"))
        {
            trimmedFormat = format[..^2];
            return true;
        }

        trimmedFormat = null;
        return false;
    }

    static void ReplaceInner(StringBuilder builder, string format, Counter counter, CultureInfo culture, TryConvert tryConvertDate)
    {
        if (!counter.ScrubDateTimes)
        {
            return;
        }

        var (max, min) = DateFormatLengthCalculator.GetLength(format, culture);

        if (builder.Length < min)
        {
            return;
        }

        if (min == max)
        {
            ReplaceFixedLength(builder, format, counter, culture, tryConvertDate, max);

            return;
        }

        ReplaceVariableLength(builder, format, counter, culture, tryConvertDate, max, min);
    }

    static void ReplaceVariableLength(StringBuilder builder, string format, Counter counter, CultureInfo culture, TryConvert tryConvertDate, int longest, int shortest)
    {
        var value = builder.AsSpan();
        var builderIndex = 0;
        for (var index = 0; index <= value.Length; index++)
        {
            var found = false;
            for (var length = longest; length >= shortest; length--)
            {
                var end = index + length;
                if (end > value.Length)
                {
                    continue;
                }

                var slice = value.Slice(index, length);
                if (tryConvertDate(slice, format, counter, culture, out var convert))
                {
                    builder.Overwrite(convert, builderIndex, length);
                    builderIndex += convert.Length;
                    index += length - 1;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                continue;
            }

            builderIndex++;
        }
    }

    static void ReplaceFixedLength(StringBuilder builder, string format, Counter counter, CultureInfo culture, TryConvert tryConvertDate, int length)
    {
        var value = builder.AsSpan();
        var builderIndex = 0;
        var increment = length - 1;
        for (var index = 0; index <= value.Length - length; index++)
        {
            var slice = value.Slice(index, length);
            if (tryConvertDate(slice, format, counter, culture, out var convert))
            {
                builder.Overwrite(convert, builderIndex, length);
                builderIndex += convert.Length;
                index += increment;
            }
            else
            {
                builderIndex++;
            }
        }
    }
}

static class LegacyLineScrubber
{
    public static void FilterLines(this StringBuilder input, Func<string, bool> removeLine)
    {
        var theString = input.ToString();
        using var reader = new StringReader(theString);
        input.Clear();

        while (reader.ReadLine() is { } line)
        {
            if (removeLine(line))
            {
                continue;
            }

            input.AppendLineN(line);
        }

        var endsWithNewLine = theString.EndsWith('\n');
        if (input.Length > 0 && !endsWithNewLine)
        {
            input.Length -= 1;
        }
    }

    public static void RemoveEmptyLines(this StringBuilder builder)
    {
        builder.FilterLines(string.IsNullOrWhiteSpace);
        if (builder.Length > 0 &&
            builder[0] is '\n')
        {
            builder.Remove(0, 1);
        }

        if (builder.Length > 0 &&
            builder[^1] is '\n')
        {
            builder.Length--;
        }
    }

    public static void RemoveLinesContaining(this StringBuilder input, StringComparison comparison, params string[] stringToMatch) =>
        input.FilterLines(_ =>
        {
            foreach (var toMatch in stringToMatch)
            {
                if (_.Contains(toMatch, comparison))
                {
                    return true;
                }
            }

            return false;
        });

    public static void ReplaceLines(this StringBuilder input, Func<string, string?> replaceLine)
    {
        var theString = input.ToString();
        using var reader = new StringReader(theString);
        input.Clear();
        while (reader.ReadLine() is { } line)
        {
            var value = replaceLine(line);
            if (value is not null)
            {
                input.AppendLineN(value);
            }
        }

        if (input.Length > 0 &&
            !theString.EndsWith('\n'))
        {
            input.Length -= 1;
        }
    }
}
