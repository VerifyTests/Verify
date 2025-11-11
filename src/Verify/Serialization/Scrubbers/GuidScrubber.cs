static class GuidScrubber
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

        // Sort by position descending
        var orderByDescending = matches.OrderByDescending(_ => _.Index);

        // Apply matches
        foreach (var match in orderByDescending)
        {
            builder.Overwrite(match.Value, match.Index, 36);
        }
    }

    public static List<Match> FindMatches(StringBuilder builder, Counter counter)
    {
        var absolutePosition = 0;
        var matches = new List<Match>();
        Span<char> carryoverBuffer = stackalloc char[35];
        var carryoverLength = 0;
        var previousChunkAbsoluteEnd = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var chunkSpan = chunk.Span;

            // Check for GUIDs spanning from previous chunk to current chunk
            if (carryoverLength > 0)
            {
                var neededFromCurrent = 36 - carryoverLength;

                if (chunkSpan.Length >= neededFromCurrent)
                {
                    Span<char> buffer = stackalloc char[36];
                    carryoverBuffer.Slice(0, carryoverLength).CopyTo(buffer);
                    chunkSpan.Slice(0, neededFromCurrent).CopyTo(buffer.Slice(carryoverLength));

                    // Check boundary characters
                    var startPosition = previousChunkAbsoluteEnd - carryoverLength;
                    var hasValidStart = startPosition == 0 || !IsInvalidStartingChar(builder[startPosition - 1]);
                    var hasValidEnd = neededFromCurrent >= chunkSpan.Length || !IsInvalidEndingChar(chunkSpan[neededFromCurrent]);

                    if (hasValidStart && hasValidEnd &&
                        !buffer.ContainsNewline() &&
                        Guid.TryParseExact(buffer, "D", out var guid))
                    {
                        var convert = counter.Convert(guid);
                        matches.Add(new(startPosition, convert));
                    }
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

                    if (slice.ContainsNewline() ||
                        !Guid.TryParseExact(slice, "D", out var guid))
                    {
                        continue;
                    }

                    var convert = counter.Convert(guid);
                    var startReplaceIndex = absolutePosition + chunkIndex;
                    matches.Add(new(startReplaceIndex, convert));
                    chunkIndex += 35;
                }
            }

            // Save last 35 chars for next iteration
            carryoverLength = Math.Min(35, chunk.Length);
            chunkSpan.Slice(chunk.Length - carryoverLength, carryoverLength).CopyTo(carryoverBuffer);

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

    internal readonly struct Match(int index, string value)
    {
        public readonly int Index = index;
        public readonly string Value = value;
    }
}