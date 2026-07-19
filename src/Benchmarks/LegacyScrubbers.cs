// Copies of the pre-engine StringBuilder based scrubber implementations, preserved so the
// benchmarks can report before/after rows over identical corpora. Sourced from
// src/Verify/Serialization/Scrubbers (deleted when the span engine replaced them).

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