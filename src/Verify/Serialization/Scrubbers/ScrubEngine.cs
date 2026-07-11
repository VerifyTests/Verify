// The span based scrub engine. Operates on a contiguous source string, tracking the document as a
// list of chunks: scannable source slices and quarantined replacement strings. Each scrubber scans
// the remaining scannable chunks; a match splits its chunk and the replacement is never re-examined
// by other scrubbers. Assembly produces a single string (zero-copy when nothing changed) or fills a
// StringBuilder when a legacy scrubber pass follows.
static partial class ScrubEngine
{
    internal readonly struct Chunk(string text, int start, int length, bool scannable)
    {
        public readonly string Text = text;
        public readonly int Start = start;
        public readonly int Length = length;
        public readonly bool Scannable = scannable;

        public CharSpan Span => Text.AsSpan(Start, Length);

        public char First => Text[Start];

        public char Last => Text[Start + Length - 1];
    }

    public static string Run(
        string source,
        EngineScrubberSet set,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        bool applyDirectoryReplacements)
    {
        var working = Scrubber.NormalizeNewlines(source);
        var chunks = ScrubCore(working, set, counter, context, applyDirectoryReplacements);
        if (chunks == null)
        {
            return working;
        }

        return AssembleString(chunks);
    }

    public static void RunToBuilder(
        string source,
        EngineScrubberSet set,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        bool applyDirectoryReplacements,
        StringBuilder target)
    {
        var working = Scrubber.NormalizeNewlines(source);
        var chunks = ScrubCore(working, set, counter, context, applyDirectoryReplacements);
        target.Clear();
        if (chunks == null)
        {
            target.Append(working);
            return;
        }

        var total = 0;
        foreach (var chunk in chunks)
        {
            total += chunk.Length;
        }

        target.EnsureCapacity(total);
        foreach (var chunk in chunks)
        {
            target.Append(chunk.Text, chunk.Start, chunk.Length);
        }
    }

    // Returns null when nothing changed (the working source is already the result)
    static List<Chunk>? ScrubCore(
        string source,
        EngineScrubberSet set,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        bool applyDirectoryReplacements)
    {
        if (source.Length == 0)
        {
            return null;
        }

        List<Chunk>? chunks = null;
        var changed = false;

        if (set.HasLinePhase)
        {
            chunks = LinePhase(source, set);
            changed = chunks != null;
        }

        if (set.Inline.Length > 0)
        {
            chunks ??= [new(source, 0, source.Length, scannable: true)];
            foreach (var scrubber in set.Inline)
            {
                changed |= ApplyInline(chunks, scrubber, counter, context);
            }
        }

        // Path replacements are pinned last so user scrubbers always see raw paths
        if (applyDirectoryReplacements)
        {
            var pairs = DirectoryReplacements.Items;
            if (pairs.Count > 0 &&
                source.Length >= DirectoryReplacements.ShortestFindLength)
            {
                chunks ??= [new(source, 0, source.Length, scannable: true)];
                changed |= ApplyDirectoryReplacements(chunks, pairs);
            }
        }

        if (changed)
        {
            return chunks;
        }

        return null;
    }

    static bool ApplyDirectoryReplacements(List<Chunk> chunks, List<DirectoryReplacements.Pair> pairs)
    {
        var shortest = pairs[^1].Find.Length;
        var changed = false;
        var chunkIndex = 0;
        while (chunkIndex < chunks.Count)
        {
            var chunk = chunks[chunkIndex];
            if (!chunk.Scannable ||
                chunk.Length < shortest)
            {
                chunkIndex++;
                continue;
            }

            if (!TryFindDirectoryMatch(chunks, chunkIndex, pairs, shortest, out var matchStart, out var matchLength, out var replacement))
            {
                chunkIndex++;
                continue;
            }

            changed = true;
            chunkIndex = Splice(chunks, chunkIndex, matchStart, matchLength, replacement);
        }

        return changed;
    }

    static bool TryFindDirectoryMatch(
        List<Chunk> chunks,
        int chunkIndex,
        List<DirectoryReplacements.Pair> pairs,
        int shortest,
        out int matchStart,
        out int matchLength,
        out string replacement)
    {
        var span = chunks[chunkIndex].Span;
        var beforeSegment = chunkIndex > 0 ? chunks[chunkIndex - 1].Last : (char?) null;
        var afterSegment = chunkIndex + 1 < chunks.Count ? chunks[chunkIndex + 1].First : (char?) null;
        for (var position = 0; position + shortest <= span.Length; position++)
        {
            if (DirectoryReplacements.TryMatchAt(span, position, beforeSegment, afterSegment, pairs, out matchLength, out replacement))
            {
                matchStart = position;
                return true;
            }
        }

        matchStart = 0;
        matchLength = 0;
        replacement = string.Empty;
        return false;
    }

    static string AssembleString(List<Chunk> chunks)
    {
        var total = 0;
        foreach (var chunk in chunks)
        {
            total += chunk.Length;
        }

        if (total == 0)
        {
            return string.Empty;
        }

#if NET6_0_OR_GREATER
        return string.Create(
            total,
            chunks,
            static (span, chunks) =>
            {
                foreach (var chunk in chunks)
                {
                    chunk.Span.CopyTo(span);
                    span = span[chunk.Length..];
                }
            });
#else
        var buffer = new char[total];
        var position = 0;
        foreach (var chunk in chunks)
        {
            chunk.Text.CopyTo(chunk.Start, buffer, position, chunk.Length);
            position += chunk.Length;
        }

        return new(buffer);
#endif
    }

    static bool ApplyInline(
        List<Chunk> chunks,
        Scrubber scrubber,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        var changed = false;
        var effectiveMin = Math.Max(1, scrubber.MinLength);
        var chunkIndex = 0;
        while (chunkIndex < chunks.Count)
        {
            var chunk = chunks[chunkIndex];
            if (!chunk.Scannable ||
                chunk.Length < effectiveMin)
            {
                chunkIndex++;
                continue;
            }

            if (!TryFindMatch(chunks, chunkIndex, scrubber, counter, context, out var matchStart, out var matchLength, out var replacement))
            {
                chunkIndex++;
                continue;
            }

            changed = true;
            chunkIndex = Splice(chunks, chunkIndex, matchStart, matchLength, replacement);
        }

        return changed;
    }

    // Splits the chunk at chunkIndex into [prefix][replacement][suffix].
    // Returns the index of the suffix chunk (to continue scanning), or the index
    // after the inserted chunks when the match ended at the chunk end.
    static int Splice(List<Chunk> chunks, int chunkIndex, int matchStart, int matchLength, string replacement)
    {
        var chunk = chunks[chunkIndex];
        chunks.RemoveAt(chunkIndex);
        var insertIndex = chunkIndex;
        if (matchStart > 0)
        {
            chunks.Insert(insertIndex, new(chunk.Text, chunk.Start, matchStart, scannable: true));
            insertIndex++;
        }

        if (replacement.Length > 0)
        {
            chunks.Insert(insertIndex, new(replacement, 0, replacement.Length, scannable: false));
            insertIndex++;
        }

        var suffixStart = matchStart + matchLength;
        var suffixLength = chunk.Length - suffixStart;
        if (suffixLength > 0)
        {
            chunks.Insert(insertIndex, new(chunk.Text, chunk.Start + suffixStart, suffixLength, scannable: true));
        }

        return insertIndex;
    }

    static bool TryFindMatch(
        List<Chunk> chunks,
        int chunkIndex,
        Scrubber scrubber,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchStart,
        out int matchLength,
        out string replacement)
    {
        switch (scrubber.Kind)
        {
            case ScrubberKind.Replace:
                return TryFindReplaceMatch(chunks, chunkIndex, scrubber, out matchStart, out matchLength, out replacement);
            case ScrubberKind.Window:
                return TryFindWindowMatch(chunks, chunkIndex, scrubber, counter, context, out matchStart, out matchLength, out replacement);
            case ScrubberKind.Match:
                return TryFindCustomMatch(chunks, chunkIndex, scrubber, counter, context, out matchStart, out matchLength, out replacement);
            default:
                throw new($"Unexpected inline scrubber kind: {scrubber.Kind}");
        }
    }

    static bool TryFindReplaceMatch(
        List<Chunk> chunks,
        int chunkIndex,
        Scrubber scrubber,
        out int matchStart,
        out int matchLength,
        out string replacement)
    {
        var span = chunks[chunkIndex].Span;
        var pairs = scrubber.Pairs!;
        matchStart = -1;
        matchLength = 0;
        replacement = string.Empty;

        // Pairs are ordered longest first, so at equal positions the longest Find wins
        foreach (var (find, pairReplacement) in pairs)
        {
            if (find.Length > span.Length)
            {
                continue;
            }

            var searchFrom = 0;
            while (searchFrom + find.Length <= span.Length)
            {
                var found = span[searchFrom..].IndexOf(find.AsSpan(), scrubber.Comparison);
                if (found < 0)
                {
                    break;
                }

                var index = searchFrom + found;
                if (matchStart >= 0 &&
                    index >= matchStart)
                {
                    // Cannot beat the current best position
                    break;
                }

                if (scrubber.RequireWordBoundary &&
                    !BoundaryOk(chunks, chunkIndex, index, find.Length))
                {
                    searchFrom = index + 1;
                    continue;
                }

                matchStart = index;
                matchLength = find.Length;
                replacement = pairReplacement;
                break;
            }
        }

        return matchStart >= 0;
    }

    static bool TryFindWindowMatch(
        List<Chunk> chunks,
        int chunkIndex,
        Scrubber scrubber,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchStart,
        out int matchLength,
        out string replacement)
    {
        var span = chunks[chunkIndex].Span;
        var min = scrubber.MinLength;
        var max = scrubber.MaxLength!.Value;
        var matcher = scrubber.WindowMatcher!;

        // Windows never contain a line break, so scan the newline-free regions
        var regionStart = 0;
        while (regionStart < span.Length)
        {
            var newlineOffset = span[regionStart..].IndexOf('\n');
            var regionEnd = newlineOffset < 0 ? span.Length : regionStart + newlineOffset;

            for (var position = regionStart; position <= regionEnd - min; position++)
            {
                if (scrubber.RequireWordBoundary &&
                    PreviousChar(chunks, chunkIndex, position) is { } before &&
                    char.IsLetterOrDigit(before))
                {
                    continue;
                }

                var upper = Math.Min(max, regionEnd - position);
                for (var length = upper; length >= min; length--)
                {
                    if (scrubber.RequireWordBoundary &&
                        NextChar(chunks, chunkIndex, position + length) is { } after &&
                        char.IsLetterOrDigit(after))
                    {
                        continue;
                    }

                    var result = matcher(span.Slice(position, length), counter, context);
                    if (result == null)
                    {
                        continue;
                    }

                    matchStart = position;
                    matchLength = length;
                    replacement = Scrubber.NormalizeNewlines(result);
                    return true;
                }
            }

            if (newlineOffset < 0)
            {
                break;
            }

            regionStart = regionEnd + 1;
        }

        matchStart = 0;
        matchLength = 0;
        replacement = string.Empty;
        return false;
    }

    static bool TryFindCustomMatch(
        List<Chunk> chunks,
        int chunkIndex,
        Scrubber scrubber,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchStart,
        out int matchLength,
        out string replacement)
    {
        var span = chunks[chunkIndex].Span;
        if (!scrubber.SegmentMatcher!(span, counter, context, out matchStart, out matchLength, out var result))
        {
            replacement = string.Empty;
            return false;
        }

        if (result == null)
        {
            throw new("SegmentMatch returned true but no replacement.");
        }

        if (matchStart < 0 ||
            matchLength <= 0 ||
            matchStart + matchLength > span.Length)
        {
            throw new($"SegmentMatch returned an invalid range. index: {matchStart}, length: {matchLength}, segment length: {span.Length}.");
        }

        if (span.Slice(matchStart, matchLength).Contains('\n'))
        {
            throw new("SegmentMatch returned a range containing a line break. Matches must not span lines.");
        }

        replacement = Scrubber.NormalizeNewlines(result);
        return true;
    }

    static bool BoundaryOk(List<Chunk> chunks, int chunkIndex, int index, int length)
    {
        if (PreviousChar(chunks, chunkIndex, index) is { } before &&
            char.IsLetterOrDigit(before))
        {
            return false;
        }

        if (NextChar(chunks, chunkIndex, index + length) is { } after &&
            char.IsLetterOrDigit(after))
        {
            return false;
        }

        return true;
    }

    // The char before the given position within the chunk. At the chunk start the
    // neighbor is the last char of the previous chunk (replacement text counts).
    static char? PreviousChar(List<Chunk> chunks, int chunkIndex, int position)
    {
        var chunk = chunks[chunkIndex];
        if (position > 0)
        {
            return chunk.Text[chunk.Start + position - 1];
        }

        if (chunkIndex > 0)
        {
            return chunks[chunkIndex - 1].Last;
        }

        return null;
    }

    // The char at the given position within the chunk. At the chunk end the
    // neighbor is the first char of the next chunk (replacement text counts).
    static char? NextChar(List<Chunk> chunks, int chunkIndex, int position)
    {
        var chunk = chunks[chunkIndex];
        if (position < chunk.Length)
        {
            return chunk.Text[chunk.Start + position];
        }

        if (chunkIndex + 1 < chunks.Count)
        {
            return chunks[chunkIndex + 1].First;
        }

        return null;
    }
}
