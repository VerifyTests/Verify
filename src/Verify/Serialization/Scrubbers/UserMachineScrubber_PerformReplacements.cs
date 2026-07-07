static partial class UserMachineScrubber
{
    static bool IsValidWrapper(char ch) =>
        !char.IsLetterOrDigit(ch);

    public static void PerformReplacements(StringBuilder builder, string find, string replace)
    {
        if (builder.Length < find.Length)
        {
            return;
        }

        var matches = FindMatches(builder, find);

        // Sort by position descending. In-place to avoid LINQ allocation
        matches.Sort((a, b) => b.CompareTo(a));

        // Apply matches
        foreach (var match in matches)
        {
            builder.Overwrite(replace, match, find.Length);
        }
    }

    static List<int> FindMatches(StringBuilder builder, string find)
    {
        var matches = new List<int>();
        var absolutePosition = 0;
        var carryoverSize = find.Length - 1;

        Span<char> carryoverBuffer = stackalloc char[carryoverSize];
        Span<char> combinedBuffer = stackalloc char[find.Length];
        var carryoverLength = 0;
        var previousChunkAbsoluteEnd = 0;

        foreach (var chunk in builder.GetChunks())
        {
            var chunkSpan = chunk.Span;

            // Check for matches spanning from previous chunk to current chunk
            if (carryoverLength > 0)
            {
                for (var carryoverIndex = 0; carryoverIndex < carryoverLength; carryoverIndex++)
                {
                    var remainingInCarryover = carryoverLength - carryoverIndex;
                    var neededFromCurrent = find.Length - remainingInCarryover;

                    if (neededFromCurrent <= 0 ||
                        neededFromCurrent > chunkSpan.Length)
                    {
                        continue;
                    }

                    // Build combined buffer
                    carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(combinedBuffer);
                    chunkSpan[..neededFromCurrent].CopyTo(combinedBuffer[remainingInCarryover..]);

                    // Check if it matches
                    if (!combinedBuffer.SequenceEqual(find))
                    {
                        continue;
                    }

                    var startPosition = previousChunkAbsoluteEnd - carryoverLength + carryoverIndex;

                    // Check preceding character
                    var validStart = startPosition == 0 ||
                                     IsValidWrapper(builder[startPosition - 1]);

                    if (!validStart)
                    {
                        continue;
                    }

                    // Check trailing character. Use the builder indexer rather
                    // than chunkSpan[neededFromCurrent], which is out of range when
                    // the match ends exactly at this chunk's boundary, so the check
                    // still works when there is a following chunk.
                    var endPosition = startPosition + find.Length;
                    var validEnd = endPosition >= builder.Length ||
                                   IsValidWrapper(builder[endPosition]);

                    if (!validEnd)
                    {
                        continue;
                    }

                    matches.Add(startPosition);
                }
            }

            // Process matches entirely within this chunk
            if (chunk.Length >= find.Length)
            {
                var chunkIndex = 0;
                while (true)
                {
                    var value = chunkSpan;
                    var searchSpan = value[chunkIndex..];
                    var foundIndex = searchSpan.IndexOf(find);
                    if (foundIndex == -1)
                    {
                        break;
                    }

                    chunkIndex += foundIndex;
                    var end = chunkIndex + find.Length;

                    if ((chunkIndex != 0 && !IsValidWrapper(value[chunkIndex - 1])) ||
                        (end != value.Length && !IsValidWrapper(value[end])))
                    {
                        chunkIndex++;
                        continue;
                    }

                    matches.Add(absolutePosition + chunkIndex);
                    chunkIndex += find.Length;
                }
            }

            // Roll the carryover forward: keep the last carryoverSize chars of
            // everything seen so far. Rebuilding it from the current chunk alone
            // drops the prefix when a chunk is shorter than the search string, so
            // a token spanning three or more chunks would never be found.
            if (chunk.Length >= carryoverSize)
            {
                chunkSpan.Slice(chunk.Length - carryoverSize, carryoverSize).CopyTo(carryoverBuffer);
                carryoverLength = carryoverSize;
            }
            else
            {
                var keep = Math.Min(carryoverLength, carryoverSize - chunk.Length);
                carryoverBuffer.Slice(carryoverLength - keep, keep).CopyTo(carryoverBuffer);
                chunkSpan.CopyTo(carryoverBuffer[keep..]);
                carryoverLength = keep + chunk.Length;
            }

            previousChunkAbsoluteEnd = absolutePosition + chunk.Length;
            absolutePosition += chunk.Length;
        }

        return matches;
    }
}