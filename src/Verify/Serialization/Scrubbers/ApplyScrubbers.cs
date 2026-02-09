// ReSharper disable RedundantSuppressNullableWarningExpression

static class ApplyScrubbers
{
    public static void ApplyForExtension(string extension, StringBuilder target, VerifySettings settings, Counter counter)
    {
        if (!settings.ScrubbersEnabled)
        {
            target.FixNewlines();
            return;
        }

        ApplySpanScrubbersForExtension(extension, target, settings, counter);

        if (settings.InstanceScrubbers != null)
        {
            foreach (var scrubber in settings.InstanceScrubbers)
            {
                scrubber(target, counter, settings.Context);
            }
        }

        if (settings.ExtensionMappedInstanceScrubbers != null &&
            settings.ExtensionMappedInstanceScrubbers.TryGetValue(extension, out var extensionBasedInstanceScrubbers))
        {
            foreach (var scrubber in extensionBasedInstanceScrubbers)
            {
                scrubber(target, counter, settings.Context);
            }
        }

        if (VerifierSettings.ExtensionMappedGlobalScrubbers.TryGetValue(extension, out var extensionBasedScrubbers))
        {
            foreach (var scrubber in extensionBasedScrubbers)
            {
                scrubber(target, counter, settings.Context);
            }
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(target, counter, settings.Context);
        }

        DirectoryReplacements.Replace(target);

        target.FixNewlines();
    }

    public static CharSpan ApplyForPropertyValue(CharSpan value, VerifySettings settings, Counter counter)
    {
        var builder = new StringBuilder(value.Length);
        builder.Append(value);
        ApplyForPropertyValue(settings, counter, builder);
        return builder.AsSpan();
    }

    public static void ApplyForPropertyValue(VerifySettings settings, Counter counter, StringBuilder builder)
    {
        if (!settings.ScrubbersEnabled)
        {
            builder.FixNewlines();
            return;
        }

        ApplySpanScrubbersForPropertyValue(builder, settings, counter);

        if (settings.InstanceScrubbers != null)
        {
            foreach (var scrubber in settings.InstanceScrubbers)
            {
                scrubber(builder, counter, settings.Context);
            }
        }

        foreach (var scrubber in VerifierSettings.GlobalScrubbers)
        {
            scrubber(builder, counter, settings.Context);
        }

        DirectoryReplacements.Replace(builder);

        builder.FixNewlines();
    }

    static void ApplySpanScrubbersForExtension(string extension, StringBuilder target, VerifySettings settings, Counter counter)
    {
        List<SpanScrubber>? scrubbers = null;

        if (settings.InstanceSpanScrubbers is {Count: > 0})
        {
            scrubbers = [..settings.InstanceSpanScrubbers];
        }

        if (settings.ExtensionMappedInstanceSpanScrubbers != null &&
            settings.ExtensionMappedInstanceSpanScrubbers.TryGetValue(extension, out var extInstanceScrubbers))
        {
            scrubbers ??= [];
            scrubbers.AddRange(extInstanceScrubbers);
        }

        if (VerifierSettings.ExtensionMappedGlobalSpanScrubbers.TryGetValue(extension, out var extGlobalScrubbers))
        {
            scrubbers ??= [];
            scrubbers.AddRange(extGlobalScrubbers);
        }

        if (VerifierSettings.GlobalSpanScrubbers.Count > 0)
        {
            scrubbers ??= [];
            scrubbers.AddRange(VerifierSettings.GlobalSpanScrubbers);
        }

        if (scrubbers is {Count: > 0})
        {
            ApplySpanScrubbers(target, scrubbers, counter);
        }
    }

    static void ApplySpanScrubbersForPropertyValue(StringBuilder target, VerifySettings settings, Counter counter)
    {
        List<SpanScrubber>? scrubbers = null;

        if (settings.InstanceSpanScrubbers is {Count: > 0})
        {
            scrubbers = [..settings.InstanceSpanScrubbers];
        }

        if (VerifierSettings.GlobalSpanScrubbers.Count > 0)
        {
            scrubbers ??= [];
            scrubbers.AddRange(VerifierSettings.GlobalSpanScrubbers);
        }

        if (scrubbers is {Count: > 0})
        {
            ApplySpanScrubbers(target, scrubbers, counter);
        }
    }

    static void ApplySpanScrubbers(StringBuilder target, List<SpanScrubber> scrubbers, Counter counter)
    {
        foreach (var scrubber in scrubbers)
        {
            ApplySingleSpanScrubber(target, scrubber, counter);
        }
    }

    static void ApplySingleSpanScrubber(StringBuilder target, SpanScrubber scrubber, Counter counter)
    {
        var input = target.ToString();
        var inputSpan = input.AsSpan();
        var output = new StringBuilder(input.Length);
        var pos = 0;

        while (pos < inputSpan.Length)
        {
            var remaining = inputSpan.Length - pos;
            var maxLen = scrubber.MaxLength ?? remaining;
            var minLen = scrubber.MinLength ?? 1;

            if (maxLen > remaining)
            {
                maxLen = remaining;
            }

            if (minLen > remaining)
            {
                output.Append(inputSpan[pos..]);
                break;
            }

            var matched = false;

            for (var length = maxLen; length >= minLen; length--)
            {
                var slice = inputSpan.Slice(pos, length);
                var result = scrubber.TryConvert(slice, counter);
                if (result.Matched)
                {
                    output.Append(result.Replacement);
                    pos += length;
                    matched = true;
                    break;
                }
            }

            if (!matched)
            {
                output.Append(inputSpan[pos]);
                pos++;
            }
        }

        target.Clear();
        target.Append(output);
    }

    static string CleanPath(string directory) =>
        directory.TrimEnd('/', '\\');
}
