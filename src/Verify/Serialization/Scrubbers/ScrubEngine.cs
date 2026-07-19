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

    // target must already contain source, so an unchanged result leaves it untouched
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
        if (chunks == null)
        {
            if (ReferenceEquals(working, source))
            {
                return;
            }

            target.Clear();
            target.Append(working);
            return;
        }

        target.Clear();

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

        foreach (var registered in set.Inline)
        {
            // A gated off built-in (inline dates or guids when the corresponding
            // scrubbing is disabled) is skipped for the whole scrub rather than
            // being probed at every candidate window
            if (registered.Gate is { } gate &&
                !gate(counter))
            {
                continue;
            }

            // A scrubber registered without an explicit culture resolves here to
            // the instance built for the culture in effect for this scrub
            var scrubber = registered.Resolve();

            chunks ??= [new(source, 0, source.Length, scannable: true)];
            if (ApplyInline(chunks, scrubber, counter, context) is { } applied)
            {
                chunks = applied;
                changed = true;
            }
        }

        // Path replacements are pinned last so user scrubbers always see raw paths
        if (applyDirectoryReplacements)
        {
            var pairs = DirectoryReplacements.Items;
            if (pairs.Count > 0)
            {
                if (chunks == null)
                {
                    // Nothing has run, so the source is still the whole document
                    if (source.Length >= DirectoryReplacements.ShortestFindLength)
                    {
                        chunks = [new(source, 0, source.Length, scannable: true)];
                        if (ApplyDirectoryReplacements(chunks, pairs) is { } replaced)
                        {
                            chunks = replaced;
                            changed = true;
                        }
                    }
                }
                else
                {
                    // A scrubber may have grown the document past the shortest find,
                    // so the pre-scrub length cannot gate this. ApplyDirectoryReplacements
                    // skips any chunk that is too short on its own.
                    if (ApplyDirectoryReplacements(chunks, pairs) is { } replaced)
                    {
                        chunks = replaced;
                        changed = true;
                    }
                }
            }
        }

        if (changed)
        {
            return chunks;
        }

        return null;
    }

    // Rebuilds the list, as ApplyInline does. Path replacements are never empty, so
    // no join can appear and the result needs no coalescing.
    static List<Chunk>? ApplyDirectoryReplacements(List<Chunk> chunks, List<DirectoryReplacements.Pair> pairs)
    {
        var shortest = pairs[^1].Find.Length;
        List<Chunk>? output = null;

        for (var index = 0; index < chunks.Count; index++)
        {
            var chunk = chunks[index];
            if (!chunk.Scannable ||
                chunk.Length < shortest)
            {
                output?.Add(chunk);
                continue;
            }

            var after = index + 1 < chunks.Count ? chunks[index + 1].First : (char?) null;
            var offset = 0;
            while (chunk.Length - offset >= shortest)
            {
                var span = chunk.Text.AsSpan(chunk.Start + offset, chunk.Length - offset);
                var before = PrecedingChar(output, chunks, index);
                if (!TryFindDirectoryMatch(span, before, after, pairs, shortest, out var matchStart, out var matchLength, out var replacement))
                {
                    break;
                }

                output ??= CopyUpTo(chunks, index);
                if (matchStart > 0)
                {
                    output.Add(new(chunk.Text, chunk.Start + offset, matchStart, scannable: true));
                }

                output.Add(new(replacement, 0, replacement.Length, scannable: false));
                offset += matchStart + matchLength;
            }

            if (output != null)
            {
                var remaining = chunk.Length - offset;
                if (remaining > 0)
                {
                    output.Add(new(chunk.Text, chunk.Start + offset, remaining, scannable: true));
                }
            }
        }

        return output;
    }

    static bool TryFindDirectoryMatch(
        CharSpan span,
        char? beforeSegment,
        char? afterSegment,
        List<DirectoryReplacements.Pair> pairs,
        int shortest,
        out int matchStart,
        out int matchLength,
        out string replacement)
    {
        var anchors = DirectoryReplacements.ItemAnchors.AsSpan();
        for (var position = 0; position + shortest <= span.Length; position++)
        {
            // Skip to the next candidate first char
            var skip = span[position..].IndexOfAny(anchors);
            if (skip < 0)
            {
                break;
            }

            position += skip;
            if (position + shortest > span.Length)
            {
                break;
            }

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

    // Rebuilds the list instead of splicing in place. An in place remove and insert
    // shifts every element after the match, so a pass over a list that an earlier
    // scrubber already fragmented would cost O(matches x chunks).
    // Returns null when nothing matched, leaving the caller's list untouched and
    // unallocated.
    static List<Chunk>? ApplyInline(
        List<Chunk> chunks,
        Scrubber scrubber,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        var effectiveMin = Math.Max(1, scrubber.MinLength);
        List<Chunk>? output = null;
        var deleted = false;

        for (var index = 0; index < chunks.Count; index++)
        {
            var chunk = chunks[index];
            if (!chunk.Scannable ||
                chunk.Length < effectiveMin)
            {
                output?.Add(chunk);
                continue;
            }

            // Chunks past this one are untouched, so the following neighbor is fixed
            var after = index + 1 < chunks.Count ? chunks[index + 1].First : (char?) null;
            var offset = 0;
            while (chunk.Length - offset >= effectiveMin)
            {
                var span = chunk.Text.AsSpan(chunk.Start + offset, chunk.Length - offset);
                var before = PrecedingChar(output, chunks, index);
                if (!TryFindMatch(span, before, after, scrubber, counter, context, out var matchStart, out var matchLength, out var replacement))
                {
                    break;
                }

                output ??= CopyUpTo(chunks, index);
                if (matchStart > 0)
                {
                    output.Add(new(chunk.Text, chunk.Start + offset, matchStart, scannable: true));
                }

                if (replacement.Length > 0)
                {
                    output.Add(new(replacement, 0, replacement.Length, scannable: false));
                }
                else
                {
                    deleted = true;
                }

                offset += matchStart + matchLength;
            }

            if (output != null)
            {
                var remaining = chunk.Length - offset;
                if (remaining > 0)
                {
                    output.Add(new(chunk.Text, chunk.Start + offset, remaining, scannable: true));
                }
            }
        }

        if (output == null)
        {
            return null;
        }

        if (deleted)
        {
            CoalesceScannable(output);
        }

        return output;
    }

    // The chunks before index are emitted unchanged, so they seed the rebuilt list
    static List<Chunk> CopyUpTo(List<Chunk> chunks, int index)
    {
        var output = new List<Chunk>(chunks.Count + 4);
        for (var copied = 0; copied < index; copied++)
        {
            output.Add(chunks[copied]);
        }

        return output;
    }

    // The char before the position being scanned: the last one emitted so far, or
    // the end of the preceding chunk while nothing has been emitted
    static char? PrecedingChar(List<Chunk>? output, List<Chunk> chunks, int index)
    {
        if (output != null)
        {
            return output.Count > 0 ? output[^1].Last : null;
        }

        return index > 0 ? chunks[index - 1].Last : null;
    }

    // An empty replacement quarantines nothing, so the document text on either
    // side of it is now adjacent. Merging those chunks keeps later scrubbers, and
    // the path replacement pass, able to match across the join.
    // Only called after a pass that deleted, since the line phase legitimately
    // leaves the whole document as adjacent scannable chunks and merging those
    // would materialize it for no gain.
    static void CoalesceScannable(List<Chunk> chunks)
    {
        var index = 0;
        while (index < chunks.Count - 1)
        {
            if (!chunks[index].Scannable ||
                !chunks[index + 1].Scannable)
            {
                index++;
                continue;
            }

            var end = index + 1;
            while (end + 1 < chunks.Count &&
                   chunks[end + 1].Scannable)
            {
                end++;
            }

            var merged = Merge(chunks, index, end);
            chunks.RemoveRange(index, end - index + 1);
            chunks.Insert(index, merged);
            index++;
        }
    }

    static Chunk Merge(List<Chunk> chunks, int start, int end)
    {
        var first = chunks[start];
        var total = first.Length;
        var contiguous = true;
        for (var index = start + 1; index <= end; index++)
        {
            var chunk = chunks[index];
            var previous = chunks[index - 1];
            if (!ReferenceEquals(chunk.Text, previous.Text) ||
                previous.Start + previous.Length != chunk.Start)
            {
                contiguous = false;
            }

            total += chunk.Length;
        }

        // Slices of the same string that are already adjacent need no allocation
        if (contiguous)
        {
            return new(first.Text, first.Start, total, scannable: true);
        }

        var builder = new StringBuilder(total);
        for (var index = start; index <= end; index++)
        {
            var chunk = chunks[index];
            builder.Append(chunk.Text, chunk.Start, chunk.Length);
        }

        return new(builder.ToString(), 0, total, scannable: true);
    }

    static bool TryFindMatch(
        CharSpan span,
        char? before,
        char? after,
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
                return TryFindReplaceMatch(span, before, after, scrubber, out matchStart, out matchLength, out replacement);
            case ScrubberKind.Window:
                return TryFindWindowMatch(span, before, after, scrubber, counter, context, out matchStart, out matchLength, out replacement);
            case ScrubberKind.Match:
                return TryFindCustomMatch(span, scrubber, counter, context, out matchStart, out matchLength, out replacement);
            default:
                throw new($"Unexpected inline scrubber kind: {scrubber.Kind}");
        }
    }

    static bool TryFindReplaceMatch(
        CharSpan span,
        char? before,
        char? after,
        Scrubber scrubber,
        out int matchStart,
        out int matchLength,
        out string replacement)
    {
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
                    !BoundaryOk(span, before, after, index, find.Length))
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
        CharSpan span,
        char? before,
        char? after,
        Scrubber scrubber,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchStart,
        out int matchLength,
        out string replacement)
    {
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
                if (scrubber.Anchor != WindowAnchor.None)
                {
                    // A match can only start where the anchor appears at the fixed
                    // offset, so jump to the next candidate
                    position = NextAnchoredPosition(span, position, regionEnd, scrubber);
                    if (position > regionEnd - min)
                    {
                        break;
                    }
                }

                if (scrubber.RequireWordBoundary &&
                    PreviousChar(span, before, position) is { } precedingChar &&
                    char.IsLetterOrDigit(precedingChar))
                {
                    continue;
                }

                var upper = Math.Min(max, regionEnd - position);
                for (var length = upper; length >= min; length--)
                {
                    if (scrubber.RequireWordBoundary &&
                        NextChar(span, after, position + length) is { } followingChar &&
                        char.IsLetterOrDigit(followingChar))
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

#if !NET8_0_OR_GREATER
    static string asciiDigits = "0123456789";
#endif

    // The next position at or after the given one where the scrubber's anchor
    // appears at its offset from the window start. Returns past regionEnd when no
    // candidate remains.
    static int NextAnchoredPosition(CharSpan span, int position, int regionEnd, Scrubber scrubber)
    {
        var searchFrom = position + scrubber.AnchorOffset;
        if (searchFrom >= regionEnd)
        {
            return regionEnd;
        }

        var region = span[searchFrom..regionEnd];
        int found;
        if (scrubber.Anchor == WindowAnchor.Char)
        {
            found = region.IndexOf(scrubber.AnchorChar);
        }
        else
        {
            // ASCII only on every target: char.IsDigit would also accept other
            // Unicode digits, so the same input would anchor differently per
            // target framework while sharing one verified file
#if NET8_0_OR_GREATER
            found = region.IndexOfAnyInRange('0', '9');
#else
            found = region.IndexOfAny(asciiDigits.AsSpan());
#endif
        }

        if (found < 0)
        {
            return regionEnd;
        }

        return searchFrom + found - scrubber.AnchorOffset;
    }

    static bool TryFindCustomMatch(
        CharSpan span,
        Scrubber scrubber,
        Counter counter,
        IReadOnlyDictionary<string, object> context,
        out int matchStart,
        out int matchLength,
        out string replacement)
    {
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

    static bool BoundaryOk(CharSpan span, char? before, char? after, int index, int length)
    {
        if (PreviousChar(span, before, index) is { } precedingChar &&
            char.IsLetterOrDigit(precedingChar))
        {
            return false;
        }

        if (NextChar(span, after, index + length) is { } followingChar &&
            char.IsLetterOrDigit(followingChar))
        {
            return false;
        }

        return true;
    }

    // The char before the given position within the span. At the span start the
    // neighbor is the one preceding it in the document (replacement text counts).
    static char? PreviousChar(CharSpan span, char? before, int position)
    {
        if (position > 0)
        {
            return span[position - 1];
        }

        return before;
    }

    // The char at the given position within the span. At the span end the neighbor
    // is the one following it in the document (replacement text counts).
    static char? NextChar(CharSpan span, char? after, int position)
    {
        if (position < span.Length)
        {
            return span[position];
        }

        return after;
    }
}
