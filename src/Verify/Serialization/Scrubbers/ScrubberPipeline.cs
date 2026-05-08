// ReSharper disable RedundantSuppressNullableWarningExpression

namespace VerifyTests;

static class ScrubberPipeline
{
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

        Apply(target, content, patterns, lines, counter, settings.Context);
        target.FixNewlines();
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

        Apply(output, content, patterns, lines, counter, settings.Context);
        output.FixNewlines();
        return output.ToString();
    }

    static void Apply(
        StringBuilder target,
        List<ContentScrubber>? contentScrubbers,
        List<PatternScrubber>? patternScrubbers,
        List<LineScrubber>? lineScrubbers,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        if (contentScrubbers is { Count: > 0 })
        {
            ApplyContentScrubbers(target, contentScrubbers, counter, context);
        }

        if (patternScrubbers is { Count: > 0 })
        {
            ApplyPatternScrubbers(target, patternScrubbers, counter, context);
        }

        if (lineScrubbers is { Count: > 0 })
        {
            ApplyLineScrubbers(target, lineScrubbers, counter, context);
        }
    }

    static void ApplyContentScrubbers(
        StringBuilder target,
        List<ContentScrubber> contentScrubbers,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        var swap = new StringBuilder(target.Length);
        foreach (var scrubber in contentScrubbers)
        {
            swap.Clear();
            var input = target.ToString();
            scrubber.Process(input.AsSpan(), swap, counter, context);
            target.Clear();
            target.Append(swap);
        }
    }

    static void ApplyPatternScrubbers(
        StringBuilder target,
        List<PatternScrubber> patternScrubbers,
        Counter counter,
        IReadOnlyDictionary<string, object> context)
    {
        patternScrubbers.Sort(ComparePatterns);

        var source = target.ToString();
        var chunks = new List<ScrubberChunk>();
        PatternWalker.Walk(source.AsSpan(), patternScrubbers, counter, context, chunks);

        target.Clear();
        PatternWalker.Stitch(source.AsSpan(), chunks, target);
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
        var input = target.ToString();
        target.Clear();

        var lineStart = 0;
        var hasContent = false;
        var i = 0;
        while (i <= input.Length)
        {
            int lineEnd;
            int nextStart;
            if (i == input.Length)
            {
                lineEnd = i;
                nextStart = i + 1;
            }
            else if (input[i] == '\r')
            {
                lineEnd = i;
                nextStart = (i + 1 < input.Length && input[i + 1] == '\n') ? i + 2 : i + 1;
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

            var line = input.AsSpan(lineStart, lineEnd - lineStart);
            var processed = (string?) line.ToString();
            foreach (var scrubber in lineScrubbers)
            {
                processed = scrubber.Process(processed.AsSpan(), counter, context);
                if (processed is null)
                {
                    break;
                }
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
