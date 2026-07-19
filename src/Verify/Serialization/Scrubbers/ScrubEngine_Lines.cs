// The line phase: walks the (already newline-normalized) source once, applying line drops first
// (needle, whitespace, and predicate based) then line transforms in registration order. Drops always
// evaluate the raw line; transform output becomes fresh scannable source for the inline phase.
// A transform that returns line breaks produces several lines, and the transforms after it see each
// of those on its own, since a line transform is defined over a single line.
// Join semantics replicate the legacy StringReader based line scrubbers: lines are joined with \n,
// the trailing newline is preserved only when the original ended with one, and RemoveEmptyLines
// additionally trims the trailing newline.
static partial class ScrubEngine
{
    static string newlineText = "\n";

    readonly struct LineItem(int start, int end, string? fresh)
    {
        // Source slice [Start, End) when Fresh is null, otherwise Fresh is replacement line text
        public readonly int Start = start;
        public readonly int End = end;
        public readonly string? Fresh = fresh;

        public int Length => Fresh?.Length ?? End - Start;
    }

    // Returns the chunks representing the document after line drops and transforms,
    // or null when no line changed.
    static List<Chunk>? LinePhase(string source, EngineScrubberSet set)
    {
        var endsWithNewline = source[^1] == '\n';
        var items = new List<LineItem>();
        var changed = false;

        // Coalesce runs of adjacent untouched kept lines (including their inner
        // separators) into single items
        int? runStart = null;
        var runEnd = 0;

        void FlushRun()
        {
            if (runStart is { } start)
            {
                items.Add(new(start, runEnd, null));
                runStart = null;
            }
        }

        var position = 0;
        while (position < source.Length)
        {
            var newlineIndex = source.IndexOf('\n', position);
            var lineEnd = newlineIndex < 0 ? source.Length : newlineIndex;
            var lineSpan = source.AsSpan(position, lineEnd - position);

            // Materialized at most once per line, shared by all string based scrubbers
            string? lineString = null;

            if (IsDropped(lineSpan, ref lineString, set.LineDrops))
            {
                changed = true;
                FlushRun();
            }
            else
            {
                var (removed, current) = ApplyTransforms(lineSpan, ref lineString, set.LineTransforms);
                if (removed)
                {
                    changed = true;
                    FlushRun();
                }
                else if (current != null &&
                         !lineSpan.SequenceEqual(current.AsSpan()))
                {
                    changed = true;
                    FlushRun();
                    items.Add(new(0, 0, current));
                }
                else
                {
                    runStart ??= position;
                    runEnd = lineEnd;
                }
            }

            if (newlineIndex < 0)
            {
                break;
            }

            position = newlineIndex + 1;
        }

        FlushRun();

        // RemoveEmptyLines trims the trailing newline even when no line was dropped
        if (set.TrimOuterEmptyLines &&
            endsWithNewline &&
            items.Count > 0)
        {
            changed = true;
        }

        if (!changed)
        {
            return null;
        }

        var chunks = new List<Chunk>(items.Count + 1);
        for (var index = 0; index < items.Count; index++)
        {
            var item = items[index];
            var needsSeparator = index < items.Count - 1 ||
                                 (endsWithNewline && !set.TrimOuterEmptyLines);

            if (item.Fresh is { } fresh)
            {
                // Transformed line text is fresh source, scannable by inline scrubbers
                if (fresh.Length > 0)
                {
                    chunks.Add(new(fresh, 0, fresh.Length, scannable: true));
                }

                if (needsSeparator)
                {
                    chunks.Add(new(newlineText, 0, 1, scannable: true));
                }

                continue;
            }

            // A separator is only needed when another item follows (the run ended at
            // a dropped or transformed line, so its last line was terminated) or when
            // the document ends with a newline. Either way the char after the run in
            // the source is '\n', so the separator folds into the slice.
            var length = item.End - item.Start + (needsSeparator ? 1 : 0);
            if (length > 0)
            {
                chunks.Add(new(source, item.Start, length, scannable: true));
            }
        }

        return chunks;
    }

    static bool IsDropped(CharSpan lineSpan, ref string? lineString, Scrubber[] lineDrops)
    {
        foreach (var drop in lineDrops)
        {
            switch (drop.Kind)
            {
                case ScrubberKind.LineDropNeedles:
                    if (lineSpan.Length < drop.MinLength)
                    {
                        continue;
                    }

                    foreach (var needle in drop.Needles!)
                    {
                        if (lineSpan.Contains(needle.AsSpan(), drop.Comparison))
                        {
                            return true;
                        }
                    }

                    continue;
                case ScrubberKind.LineDropEmpty:
                    if (lineSpan.IsWhiteSpace())
                    {
                        return true;
                    }

                    continue;
                case ScrubberKind.LineDropSpan:
                    if (drop.LineMatcher!(lineSpan))
                    {
                        return true;
                    }

                    continue;
                case ScrubberKind.LineDropString:
                    lineString ??= lineSpan.ToString();
                    if (drop.LineStringMatcher!(lineString))
                    {
                        return true;
                    }

                    continue;
                default:
                    throw new($"Unexpected line drop kind: {drop.Kind}");
            }
        }

        return false;
    }

    static (bool removed, string? current) ApplyTransforms(CharSpan lineSpan, ref string? lineString, Scrubber[] lineTransforms)
    {
        // The most recent replacement text; null while the line is unchanged
        string? current = null;
        for (var index = 0; index < lineTransforms.Length; index++)
        {
            var transform = lineTransforms[index];
            string output;
            switch (transform.Kind)
            {
                case ScrubberKind.LineTransformSpan:
                {
                    var result = transform.LineReplacer!(current is null ? lineSpan : current.AsSpan());
                    if (result.Kind == LineResult.RemoveKind)
                    {
                        return (true, null);
                    }

                    if (result.Kind != LineResult.ReplaceKind)
                    {
                        continue;
                    }

                    output = result.Text!;
                    break;
                }
                case ScrubberKind.LineTransformString:
                {
                    var input = current ?? (lineString ??= lineSpan.ToString());
                    var replaced = transform.LineStringReplacer!(input);
                    if (replaced == null)
                    {
                        return (true, null);
                    }

                    output = Scrubber.NormalizeNewlines(replaced);
                    break;
                }
                default:
                    throw new($"Unexpected line transform kind: {transform.Kind}");
            }

            current = output;

            // A replacement holding line breaks is several lines, and a line
            // transform is defined over one line, so the remaining transforms see
            // each produced line on its own. The pre engine pipeline did this by
            // re-reading the document between passes.
            if (output.Contains('\n'))
            {
                return ApplyToProducedLines(output, lineTransforms, index + 1);
            }
        }

        return (false, current);
    }

    // The remaining transforms over each line of a multi line replacement.
    // Splitting and joining on '\n' are exact inverses, so transforms that change
    // nothing leave the text as it was.
    static (bool removed, string? current) ApplyToProducedLines(string text, Scrubber[] lineTransforms, int startIndex)
    {
        if (startIndex == lineTransforms.Length)
        {
            return (false, text);
        }

        var lines = new List<string>(text.Split('\n'));
        for (var index = startIndex; index < lineTransforms.Length; index++)
        {
            var transform = lineTransforms[index];
            var next = new List<string>(lines.Count);
            foreach (var line in lines)
            {
                if (!TryTransformLine(transform, line, out var replacement))
                {
                    continue;
                }

                if (replacement == null)
                {
                    next.Add(line);
                    continue;
                }

                if (replacement.Contains('\n'))
                {
                    next.AddRange(replacement.Split('\n'));
                    continue;
                }

                next.Add(replacement);
            }

            lines = next;
        }

        if (lines.Count == 0)
        {
            return (true, null);
        }

        return (false, string.Join("\n", lines));
    }

    // False when the line is removed. replacement is null when it is unchanged.
    static bool TryTransformLine(Scrubber transform, string line, out string? replacement)
    {
        switch (transform.Kind)
        {
            case ScrubberKind.LineTransformSpan:
            {
                var result = transform.LineReplacer!(line.AsSpan());
                if (result.Kind == LineResult.RemoveKind)
                {
                    replacement = null;
                    return false;
                }

                replacement = result.Kind == LineResult.ReplaceKind ? result.Text! : null;
                return true;
            }
            case ScrubberKind.LineTransformString:
            {
                var output = transform.LineStringReplacer!(line);
                if (output == null)
                {
                    replacement = null;
                    return false;
                }

                replacement = Scrubber.NormalizeNewlines(output);
                return true;
            }
            default:
                throw new($"Unexpected line transform kind: {transform.Kind}");
        }
    }
}
