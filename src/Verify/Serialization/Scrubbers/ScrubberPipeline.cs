// ReSharper disable RedundantSuppressNullableWarningExpression

static class ScrubberPipeline
{
    [ThreadStatic]
    static List<ScrubberChunk>? threadChunks;

    [ThreadStatic]
    static StringBuilder? threadSwap;

    public static void ApplyForExtension(string extension, StringBuilder target, VerifySettings settings, Counter counter)
    {
        if (!settings.ScrubbersEnabled)
        {
            FixNewlinesIfNeeded(target);
            return;
        }

        var content = CollectContent(settings, extension);
        var patterns = CollectPatterns(settings, extension);
        var lines = CollectLines(settings, extension);
        var context = settings.Context;

        // Fast path: only pattern scrubbers (the common JSON property / log line case).
        if (content is null && lines is null)
        {
            if (patterns is null)
            {
                FixNewlinesIfNeeded(target);
                return;
            }

            ApplyPatternScrubbers(target, patterns, counter, context);
            FixNewlinesIfNeeded(target);
            return;
        }

        if (content is not null)
        {
            ApplyContentScrubbers(target, content, counter, context);
        }

        if (patterns is not null)
        {
            ApplyPatternScrubbers(target, patterns, counter, context);
        }

        if (lines is not null)
        {
            ApplyLineScrubbers(target, lines, counter, context);
        }

        FixNewlinesIfNeeded(target);
    }

    public static string ApplyForPropertyValue(CharSpan value, VerifySettings settings, Counter counter)
    {
        if (!settings.ScrubbersEnabled)
        {
            return value.IndexOf('\r') >= 0 ? Normalize(value) : value.ToString();
        }

        var content = CollectContentNoExtension(settings);
        var patterns = CollectPatternsNoExtension(settings);
        var lines = CollectLinesNoExtension(settings);

        if (content is null && patterns is null && lines is null)
        {
            return value.IndexOf('\r') >= 0 ? Normalize(value) : value.ToString();
        }

        var output = new StringBuilder(value.Length);
        output.Append(value);
        var context = settings.Context;

        if (content is not null)
        {
            ApplyContentScrubbers(output, content, counter, context);
        }

        if (patterns is not null)
        {
            ApplyPatternScrubbers(output, patterns, counter, context);
        }

        if (lines is not null)
        {
            ApplyLineScrubbers(output, lines, counter, context);
        }

        FixNewlinesIfNeeded(output);
        return output.ToString();
    }

    static string Normalize(CharSpan value)
    {
        var builder = new StringBuilder(value.Length);
        builder.Append(value);
        builder.FixNewlines();
        return builder.ToString();
    }

    static void FixNewlinesIfNeeded(StringBuilder target)
    {
        // Single scan: only normalize when \r is actually present, and start replacements
        // at the first \r so any clean prefix is skipped.
        var first = IndexOfCarriageReturn(target);
        if (first < 0)
        {
            return;
        }

        target.Replace("\r\n", "\n", first, target.Length - first);
        target.Replace('\r', '\n', first, target.Length - first);
    }

    static void ApplyContentScrubbers(
        StringBuilder target,
        List<ContentScrubber> contentScrubbers,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        var swap = RentSwap();
        var sourcePool = ArrayPool<char>.Shared;
        try
        {
            foreach (var scrubber in contentScrubbers)
            {
                swap.Clear();
                var length = target.Length;
                var sourceBuffer = sourcePool.Rent(length);
                target.CopyTo(0, sourceBuffer, 0, length);
                scrubber.Process(sourceBuffer.AsSpan(0, length), swap, counter, context);
                sourcePool.Return(sourceBuffer);

                target.Clear();
                target.Append(swap);
            }
        }
        finally
        {
            ReturnSwap(swap);
        }
    }

    static void ApplyPatternScrubbers(
        StringBuilder target,
        List<PatternScrubber> patternScrubbers,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        if (patternScrubbers.Count > 1)
        {
            patternScrubbers.Sort(ComparePatterns);
        }

        var length = target.Length;
        if (length == 0)
        {
            return;
        }

        var sourcePool = ArrayPool<char>.Shared;
        var sourceBuffer = sourcePool.Rent(length);
        target.CopyTo(0, sourceBuffer, 0, length);
        var sourceSpan = sourceBuffer.AsSpan(0, length);

        var chunks = RentChunks();
        try
        {
            PatternWalker.Walk(sourceSpan, patternScrubbers, counter, context, chunks);

            var hasReplacement = false;
            foreach (var chunk in chunks)
            {
                if (chunk.Replacement is not null)
                {
                    hasReplacement = true;
                    break;
                }
            }

            if (!hasReplacement)
            {
                return;
            }

            target.Clear();
            PatternWalker.Stitch(sourceSpan, chunks, target);
        }
        finally
        {
            ReturnChunks(chunks);
            sourcePool.Return(sourceBuffer);
        }
    }

    static int ComparePatterns(PatternScrubber a, PatternScrubber b)
    {
        var maxCmp = b.MaxLength.CompareTo(a.MaxLength);
        if (maxCmp != 0)
        {
            return maxCmp;
        }

        return b.MinLength.CompareTo(a.MinLength);
    }

    static void ApplyLineScrubbers(
        StringBuilder target,
        List<LineScrubber> lineScrubbers,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        var length = target.Length;
        var sourcePool = ArrayPool<char>.Shared;
        var sourceBuffer = sourcePool.Rent(length);
        try
        {
            target.CopyTo(0, sourceBuffer, 0, length);
            var input = sourceBuffer.AsSpan(0, length);
            target.Clear();

            var lineStart = 0;
            var hasContent = false;
            while (lineStart <= length)
            {
                int lineEnd;
                int nextStart;
                if (lineStart == length)
                {
                    lineEnd = length;
                    nextStart = length + 1;
                }
                else
                {
                    var newlineOffset = input.Slice(lineStart).IndexOfAny('\r', '\n');
                    if (newlineOffset < 0)
                    {
                        lineEnd = length;
                        nextStart = length + 1;
                    }
                    else
                    {
                        var newlineIdx = lineStart + newlineOffset;
                        lineEnd = newlineIdx;
                        nextStart = input[newlineIdx] == '\r' &&
                                    newlineIdx + 1 < length &&
                                    input[newlineIdx + 1] == '\n'
                            ? newlineIdx + 2
                            : newlineIdx + 1;
                    }
                }

                CharSpan current = input.Slice(lineStart, lineEnd - lineStart);
                var drop = false;
                foreach (var scrubber in lineScrubbers)
                {
                    if (!scrubber.Process(current, counter, context, out var replacement))
                    {
                        drop = true;
                        break;
                    }

                    current = replacement;
                }

                if (!drop)
                {
                    if (hasContent)
                    {
                        target.Append('\n');
                    }

                    target.Append(current);
                    hasContent = true;
                }

                lineStart = nextStart;
            }
        }
        finally
        {
            sourcePool.Return(sourceBuffer);
        }
    }

    static List<ScrubberChunk> RentChunks()
    {
        var list = threadChunks;
        if (list is null)
        {
            return new(8);
        }

        threadChunks = null;
        list.Clear();
        return list;
    }

    static void ReturnChunks(List<ScrubberChunk> list)
    {
        // Drop the reference to any large per-call growth before recycling.
        if (list.Capacity > 256)
        {
            return;
        }

        threadChunks = list;
    }

    static StringBuilder RentSwap()
    {
        var sb = threadSwap;
        if (sb is null)
        {
            return new(256);
        }

        threadSwap = null;
        sb.Clear();
        return sb;
    }

    static void ReturnSwap(StringBuilder sb)
    {
        if (sb.Capacity > 64 * 1024)
        {
            return;
        }

        threadSwap = sb;
    }

    static int IndexOfCarriageReturn(StringBuilder builder)
    {
        var offset = 0;
        foreach (var chunk in builder.GetChunks())
        {
            var index = chunk.Span.IndexOf('\r');
            if (index >= 0)
            {
                return offset + index;
            }

            offset += chunk.Length;
        }

        return -1;
    }

    static List<ContentScrubber>? CollectContent(VerifySettings settings, string extension)
    {
        var instance = NonEmpty(settings.InstanceContentScrubbers);
        var extInstance = NonEmpty(LookupExt(settings.ExtensionMappedInstanceContentScrubbers, extension));
        var extGlobal = NonEmpty(LookupExt(VerifierSettings.ExtensionMappedGlobalContentScrubbers, extension));
        var global = NonEmpty(VerifierSettings.GlobalContentScrubbers);

        return CombineSources(instance, extInstance, extGlobal, global);
    }

    static List<PatternScrubber>? CollectPatterns(VerifySettings settings, string extension)
    {
        var instance = NonEmpty(settings.InstancePatternScrubbers);
        var extInstance = NonEmpty(LookupExt(settings.ExtensionMappedInstancePatternScrubbers, extension));
        var extGlobal = NonEmpty(LookupExt(VerifierSettings.ExtensionMappedGlobalPatternScrubbers, extension));
        var global = NonEmpty(VerifierSettings.GlobalPatternScrubbers);
        var hasDirReplacement = DirectoryReplacements.Items.Count > 0;

        var combined = CombineSources(instance, extInstance, extGlobal, global);
        if (!hasDirReplacement)
        {
            return combined;
        }

        // Directory replacements participate as a synthetic pattern; sort handles ordering.
        if (combined is null)
        {
            return [DirectoryReplacementsPatternScrubber.Instance];
        }

        // Combined may be one of the source lists (zero-alloc fast path); copy before mutating.
        if (combined == instance || combined == extInstance || combined == extGlobal || combined == global)
        {
            var copy = new List<PatternScrubber>(combined.Count + 1);
            copy.AddRange(combined);
            copy.Add(DirectoryReplacementsPatternScrubber.Instance);
            return copy;
        }

        combined.Add(DirectoryReplacementsPatternScrubber.Instance);
        return combined;
    }

    static List<LineScrubber>? CollectLines(VerifySettings settings, string extension)
    {
        var instance = NonEmpty(settings.InstanceLineScrubbers);
        var extInstance = NonEmpty(LookupExt(settings.ExtensionMappedInstanceLineScrubbers, extension));
        var extGlobal = NonEmpty(LookupExt(VerifierSettings.ExtensionMappedGlobalLineScrubbers, extension));
        var global = NonEmpty(VerifierSettings.GlobalLineScrubbers);

        return CombineSources(instance, extInstance, extGlobal, global);
    }

    static List<ContentScrubber>? CollectContentNoExtension(VerifySettings settings) =>
        CombineSources(NonEmpty(settings.InstanceContentScrubbers), null, null, NonEmpty(VerifierSettings.GlobalContentScrubbers));

    static List<PatternScrubber>? CollectPatternsNoExtension(VerifySettings settings)
    {
        var instance = NonEmpty(settings.InstancePatternScrubbers);
        var global = NonEmpty(VerifierSettings.GlobalPatternScrubbers);
        var hasDirReplacement = DirectoryReplacements.Items.Count > 0;

        var combined = CombineSources(instance, null, null, global);
        if (!hasDirReplacement)
        {
            return combined;
        }

        if (combined is null)
        {
            return [DirectoryReplacementsPatternScrubber.Instance];
        }

        if (combined == instance || combined == global)
        {
            var copy = new List<PatternScrubber>(combined.Count + 1);
            copy.AddRange(combined);
            copy.Add(DirectoryReplacementsPatternScrubber.Instance);
            return copy;
        }

        combined.Add(DirectoryReplacementsPatternScrubber.Instance);
        return combined;
    }

    static List<LineScrubber>? CollectLinesNoExtension(VerifySettings settings) =>
        CombineSources(NonEmpty(settings.InstanceLineScrubbers), null, null, NonEmpty(VerifierSettings.GlobalLineScrubbers));

    static List<T>? NonEmpty<T>(List<T>? source) =>
        source is { Count: > 0 } ? source : null;

    static List<T>? LookupExt<T>(Dictionary<string, List<T>>? map, string extension) =>
        map != null && map.TryGetValue(extension, out var list) ? list : null;

    static List<T>? CombineSources<T>(List<T>? a, List<T>? b, List<T>? c, List<T>? d)
    {
        // Count populated sources; if only one, return it directly to avoid allocating a copy.
        // The pipeline may sort the returned list in place, but sorts are deterministic and
        // idempotent on the user-owned source.
        var count = 0;
        List<T>? single = null;
        if (a != null) { count++; single = a; }
        if (b != null) { count++; single = b; }
        if (c != null) { count++; single = c; }
        if (d != null) { count++; single = d; }

        if (count == 0)
        {
            return null;
        }

        if (count == 1)
        {
            return single;
        }

        var total = (a?.Count ?? 0) + (b?.Count ?? 0) + (c?.Count ?? 0) + (d?.Count ?? 0);
        var result = new List<T>(total);
        if (a != null) result.AddRange(a);
        if (b != null) result.AddRange(b);
        if (c != null) result.AddRange(c);
        if (d != null) result.AddRange(d);
        return result;
    }
}
