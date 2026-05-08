// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyTests;

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
            target.FixNewlines();
            return;
        }

        var content = CollectContent(settings, extension);
        var patterns = CollectPatterns(settings, extension);
        var lines = CollectLines(settings, extension);

        var modified = Apply(target, content, patterns, lines, counter, settings.Context);
        if (modified || NeedsNewlineNormalization(target))
        {
            target.FixNewlines();
        }
    }

    public static string ApplyForPropertyValue(CharSpan value, VerifySettings settings, Counter counter)
    {
        var output = new StringBuilder(value.Length);
        output.Append(value);

        if (!settings.ScrubbersEnabled)
        {
            output.FixNewlines();
            return output.ToString();
        }

        // Property-value path uses only instance + global (no extension mapping).
        var content = CollectContentNoExtension(settings);
        var patterns = CollectPatternsNoExtension(settings);
        var lines = CollectLinesNoExtension(settings);

        var modified = Apply(output, content, patterns, lines, counter, settings.Context);
        if (modified || NeedsNewlineNormalization(output))
        {
            output.FixNewlines();
        }

        return output.ToString();
    }

    static bool Apply(
        StringBuilder target,
        List<ContentScrubber>? contentScrubbers,
        List<PatternScrubber>? patternScrubbers,
        List<LineScrubber>? lineScrubbers,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        var modified = false;

        if (contentScrubbers is { Count: > 0 })
        {
            ApplyContentScrubbers(target, contentScrubbers, counter, context);
            modified = true;
        }

        if (patternScrubbers is { Count: > 0 })
        {
            modified |= ApplyPatternScrubbers(target, patternScrubbers, counter, context);
        }

        if (lineScrubbers is { Count: > 0 })
        {
            ApplyLineScrubbers(target, lineScrubbers, counter, context);
            modified = true;
        }

        return modified;
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

    static bool ApplyPatternScrubbers(
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
            return false;
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
                // No matches; the StringBuilder content is unchanged.
                return false;
            }

            target.Clear();
            PatternWalker.Stitch(sourceSpan, chunks, target);
            return true;
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
            var i = 0;
            while (i <= length)
            {
                int lineEnd;
                int nextStart;
                if (i == length)
                {
                    lineEnd = i;
                    nextStart = i + 1;
                }
                else if (input[i] == '\r')
                {
                    lineEnd = i;
                    nextStart = (i + 1 < length && input[i + 1] == '\n') ? i + 2 : i + 1;
                }
                else if (input[i] == '\n')
                {
                    lineEnd = i;
                    nextStart = i + 1;
                }
                else
                {
                    i++;
                    continue;
                }

                var line = input.Slice(lineStart, lineEnd - lineStart);
                string? processed = null;
                var first = true;
                foreach (var scrubber in lineScrubbers)
                {
                    if (first)
                    {
                        processed = scrubber.Process(line, counter, context);
                        first = false;
                    }
                    else if (processed is not null)
                    {
                        processed = scrubber.Process(processed.AsSpan(), counter, context);
                    }

                    if (processed is null)
                    {
                        break;
                    }
                }

                if (first)
                {
                    // No scrubbers ran; defensive — shouldn't happen given outer Count > 0 check.
                    processed = line.ToString();
                }

                if (processed is not null)
                {
                    if (hasContent)
                    {
                        target.Append('\n');
                    }

                    target.Append(processed);
                    hasContent = true;
                }

                lineStart = nextStart;
                i = nextStart;
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

    static bool NeedsNewlineNormalization(StringBuilder builder)
    {
        // If the buffer already only contains '\n' line endings, FixNewlines is a no-op work pass.
        // Scan once and skip the pass when nothing needs converting.
        foreach (var chunk in builder.GetChunks())
        {
            var span = chunk.Span;
            if (span.IndexOf('\r') >= 0)
            {
                return true;
            }
        }

        return false;
    }

    static List<ContentScrubber>? CollectContent(VerifySettings settings, string extension)
    {
        List<ContentScrubber>? result = null;

        AppendNonEmpty(ref result, settings.InstanceContentScrubbers);

        if (settings.ExtensionMappedInstanceContentScrubbers != null &&
            settings.ExtensionMappedInstanceContentScrubbers.TryGetValue(extension, out var instanceForExt))
        {
            AppendNonEmpty(ref result, instanceForExt);
        }

        if (VerifierSettings.ExtensionMappedGlobalContentScrubbers.TryGetValue(extension, out var globalForExt))
        {
            AppendNonEmpty(ref result, globalForExt);
        }

        AppendNonEmpty(ref result, VerifierSettings.GlobalContentScrubbers);
        return result;
    }

    static List<PatternScrubber>? CollectPatterns(VerifySettings settings, string extension)
    {
        List<PatternScrubber>? result = null;

        AppendNonEmpty(ref result, settings.InstancePatternScrubbers);

        if (settings.ExtensionMappedInstancePatternScrubbers != null &&
            settings.ExtensionMappedInstancePatternScrubbers.TryGetValue(extension, out var instanceForExt))
        {
            AppendNonEmpty(ref result, instanceForExt);
        }

        if (VerifierSettings.ExtensionMappedGlobalPatternScrubbers.TryGetValue(extension, out var globalForExt))
        {
            AppendNonEmpty(ref result, globalForExt);
        }

        AppendNonEmpty(ref result, VerifierSettings.GlobalPatternScrubbers);

        // Always include the directory replacements scrubber as the last pattern;
        // the engine sorts by MaxLength desc so it competes naturally with other patterns.
        if (DirectoryReplacements.Items.Count > 0)
        {
            result ??= [];
            result.Add(DirectoryReplacementsPatternScrubber.Instance);
        }

        return result;
    }

    static List<LineScrubber>? CollectLines(VerifySettings settings, string extension)
    {
        List<LineScrubber>? result = null;

        AppendNonEmpty(ref result, settings.InstanceLineScrubbers);

        if (settings.ExtensionMappedInstanceLineScrubbers != null &&
            settings.ExtensionMappedInstanceLineScrubbers.TryGetValue(extension, out var instanceForExt))
        {
            AppendNonEmpty(ref result, instanceForExt);
        }

        if (VerifierSettings.ExtensionMappedGlobalLineScrubbers.TryGetValue(extension, out var globalForExt))
        {
            AppendNonEmpty(ref result, globalForExt);
        }

        AppendNonEmpty(ref result, VerifierSettings.GlobalLineScrubbers);
        return result;
    }

    static List<ContentScrubber>? CollectContentNoExtension(VerifySettings settings)
    {
        List<ContentScrubber>? result = null;
        AppendNonEmpty(ref result, settings.InstanceContentScrubbers);
        AppendNonEmpty(ref result, VerifierSettings.GlobalContentScrubbers);
        return result;
    }

    static List<PatternScrubber>? CollectPatternsNoExtension(VerifySettings settings)
    {
        List<PatternScrubber>? result = null;
        AppendNonEmpty(ref result, settings.InstancePatternScrubbers);
        AppendNonEmpty(ref result, VerifierSettings.GlobalPatternScrubbers);

        if (DirectoryReplacements.Items.Count > 0)
        {
            result ??= [];
            result.Add(DirectoryReplacementsPatternScrubber.Instance);
        }

        return result;
    }

    static List<LineScrubber>? CollectLinesNoExtension(VerifySettings settings)
    {
        List<LineScrubber>? result = null;
        AppendNonEmpty(ref result, settings.InstanceLineScrubbers);
        AppendNonEmpty(ref result, VerifierSettings.GlobalLineScrubbers);
        return result;
    }

    static void AppendNonEmpty<T>(ref List<T>? target, IReadOnlyCollection<T>? source)
    {
        if (source is null || source.Count == 0)
        {
            return;
        }

        target ??= new(source.Count);
        target.AddRange(source);
    }
}
