static partial class UserMachineScrubber
{
    static bool IsValidWrapper(char ch) =>
        ch is
            ' ' or
            '\t' or
            '\n' or
            '\r';

    public static void PerformReplacements(StringBuilder builder, string find, string replace)
    {
        if (builder.Length < find.Length)
        {
            return;
        }

        var matches = FindMatches(builder, find);

        // Sort by position descending
        var orderByDescending = matches.OrderByDescending(_ => _);

        // Apply matches
        foreach (var match in orderByDescending)
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

                    if (neededFromCurrent <= 0 || neededFromCurrent > chunkSpan.Length)
                    {
                        continue;
                    }

                    // Build combined buffer
                    carryoverBuffer.Slice(carryoverIndex, remainingInCarryover).CopyTo(combinedBuffer);
                    chunkSpan.Slice(0, neededFromCurrent).CopyTo(combinedBuffer.Slice(remainingInCarryover));

                    // Check if it matches
                    if (combinedBuffer.SequenceEqual(find))
                    {
                        var startPosition = previousChunkAbsoluteEnd - carryoverLength + carryoverIndex;

                        // Check preceding character
                        var hasValidStart = startPosition == 0 || IsValidWrapper(builder[startPosition - 1]);

                        // Check trailing character
                        var endPosition = startPosition + find.Length;
                        var hasValidEnd = endPosition >= builder.Length || IsValidWrapper(chunkSpan[neededFromCurrent]);

                        if (hasValidStart && hasValidEnd)
                        {
                            matches.Add(startPosition);
                        }
                    }
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

            // Save last N chars for next iteration
            carryoverLength = Math.Min(carryoverSize, chunk.Length);
            chunkSpan.Slice(chunk.Length - carryoverLength, carryoverLength).CopyTo(carryoverBuffer);

            previousChunkAbsoluteEnd = absolutePosition + chunk.Length;
            absolutePosition += chunk.Length;
        }

        return matches;
    }
}