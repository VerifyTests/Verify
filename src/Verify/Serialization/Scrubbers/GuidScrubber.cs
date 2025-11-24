using Match = StringBuilderChunkMatcher.Match;

static class GuidScrubber
{
    const int GuidLength = 36;

    public static void ReplaceGuids(StringBuilder builder, Counter counter)
    {
        if (!counter.ScrubGuids)
        {
            return;
        }

        //{173535ae-995b-4cc6-a74e-8cd4be57039c}
        if (builder.Length < GuidLength)
        {
            return;
        }

        var matches = FindMatches(builder, counter);
        StringBuilderChunkMatcher.ApplyMatches(builder, matches);
    }

    static List<Match> FindMatches(StringBuilder builder, Counter counter)
    {
        const int carryoverSize = GuidLength - 1;
        var absolutePosition = 0;
        var matches = new List<Match>();
        Span<char> carryoverBuffer = stackalloc char[carryoverSize];
        Span<char> buffer = stackalloc char[GuidLength];
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
                    var neededFromCurrent = GuidLength - remainingInCarryover;

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
                    matches.Add(new(startPosition, GuidLength, convert));
                }
            }

            // Process GUIDs entirely within this chunk
            if (chunk.Length >= GuidLength)
            {
                for (var chunkIndex = 0; chunkIndex < chunk.Length; chunkIndex++)
                {
                    var end = chunkIndex + GuidLength;
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

                    var slice = value.Slice(chunkIndex, GuidLength);

                    if (!Guid.TryParseExact(slice, "D", out var guid))
                    {
                        continue;
                    }

                    var convert = counter.Convert(guid);
                    var startReplaceIndex = absolutePosition + chunkIndex;
                    matches.Add(new(startReplaceIndex, GuidLength, convert));
                    chunkIndex += carryoverSize;
                }
            }

            // Save last 35 chars for next iteration
            carryoverLength = Math.Min(carryoverSize, chunk.Length);
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
}