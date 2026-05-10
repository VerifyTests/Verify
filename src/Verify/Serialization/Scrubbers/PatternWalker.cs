namespace VerifyTests;

/// <summary>
/// Single-pass scanner over an input span. At each position, tries each
/// pattern scrubber in order. On a match, emits a replacement chunk and
/// skips the consumed range; non-matches accumulate into a passthrough chunk.
/// </summary>
static class PatternWalker
{
    public static void Walk(
        CharSpan source,
        IReadOnlyList<PatternScrubber> patterns,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        List<ScrubberChunk> chunks)
    {
        if (patterns.Count == 0 || source.Length == 0)
        {
            if (source.Length > 0)
            {
                AddPassthrough(chunks, 0, source.Length);
            }

            return;
        }

        var globalMin = int.MaxValue;
        var allSingleLine = true;
        foreach (var pattern in patterns)
        {
            if (pattern.MinLength < globalMin)
            {
                globalMin = pattern.MinLength;
            }

            if (!pattern.SingleLine)
            {
                allSingleLine = false;
            }
        }

        if (source.Length < globalMin)
        {
            AddPassthrough(chunks, 0, source.Length);
            return;
        }

        if (allSingleLine)
        {
            WalkPerLine(source, patterns, counter, context, chunks, globalMin);
        }
        else
        {
            WalkRange(source, 0, source.Length, patterns, counter, context, chunks, globalMin);
        }
    }

    static void WalkPerLine(
        CharSpan source,
        IReadOnlyList<PatternScrubber> patterns,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        List<ScrubberChunk> chunks,
        int globalMin)
    {
        var lineStart = 0;
        while (lineStart < source.Length)
        {
            var remaining = source.Slice(lineStart);
            var nextTerminator = remaining.IndexOfAny('\n', '\r');
            if (nextTerminator < 0)
            {
                WalkLine(source, lineStart, source.Length, patterns, counter, context, chunks, globalMin);
                return;
            }

            var terminatorStart = lineStart + nextTerminator;
            if (terminatorStart > lineStart)
            {
                WalkLine(source, lineStart, terminatorStart, patterns, counter, context, chunks, globalMin);
            }

            var terminatorLength = 1;
            if (source[terminatorStart] == '\r' &&
                terminatorStart + 1 < source.Length &&
                source[terminatorStart + 1] == '\n')
            {
                terminatorLength = 2;
            }

            AddPassthrough(chunks, terminatorStart, terminatorLength);
            lineStart = terminatorStart + terminatorLength;
        }
    }

    static void WalkLine(
        CharSpan source,
        int start,
        int end,
        IReadOnlyList<PatternScrubber> patterns,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        List<ScrubberChunk> chunks,
        int globalMin)
    {
        if (end - start < globalMin)
        {
            AddPassthrough(chunks, start, end - start);
            return;
        }

        WalkRange(source, start, end, patterns, counter, context, chunks, globalMin);
    }

    static void WalkRange(
        CharSpan source,
        int start,
        int end,
        IReadOnlyList<PatternScrubber> patterns,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        List<ScrubberChunk> chunks,
        int globalMin)
    {
        var i = start;
        var pendingStart = start;
        while (i < end)
        {
            var remaining = end - i;
            if (remaining < globalMin)
            {
                break;
            }

            var matched = TryMatchAt(source, i, remaining, patterns, counter, context, out var matchLength, out var replacement);
            if (matched)
            {
                if (i > pendingStart)
                {
                    AddPassthrough(chunks, pendingStart, i - pendingStart);
                }

                chunks.Add(ScrubberChunk.Replace(replacement!));
                i += matchLength;
                pendingStart = i;
            }
            else
            {
                i++;
            }
        }

        if (end > pendingStart)
        {
            AddPassthrough(chunks, pendingStart, end - pendingStart);
        }
    }

    static bool TryMatchAt(
        CharSpan source,
        int position,
        int remaining,
        IReadOnlyList<PatternScrubber> patterns,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchLength,
        out string? replacement)
    {
        // Patterns are sorted by MaxLength desc; longer/more-specific wins.
        for (var p = 0; p < patterns.Count; p++)
        {
            var pattern = patterns[p];
            if (remaining < pattern.MinLength)
            {
                continue;
            }

            if (pattern.TryMatch(source, position, counter, context, out matchLength, out replacement))
            {
                return true;
            }
        }

        matchLength = 0;
        replacement = null;
        return false;
    }

    static void AddPassthrough(List<ScrubberChunk> chunks, int start, int length)
    {
        if (length == 0)
        {
            return;
        }

        if (chunks.Count > 0)
        {
            var last = chunks[^1];
            if (last.Replacement is null && last.SourceStart + last.SourceLength == start)
            {
                chunks[^1] = ScrubberChunk.Passthrough(last.SourceStart, last.SourceLength + length);
                return;
            }
        }

        chunks.Add(ScrubberChunk.Passthrough(start, length));
    }

    public static void Stitch(CharSpan source, List<ScrubberChunk> chunks, StringBuilder output)
    {
        var totalLength = 0;
        foreach (var chunk in chunks)
        {
            totalLength += chunk.Length;
        }

        if (output.Capacity < totalLength)
        {
            output.Capacity = totalLength;
        }

        foreach (var chunk in chunks)
        {
            if (chunk.Replacement is null)
            {
                output.Append(source.Slice(chunk.SourceStart, chunk.SourceLength));
            }
            else
            {
                output.Append(chunk.Replacement);
            }
        }
    }
}
