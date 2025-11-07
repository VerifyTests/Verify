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

    static string CleanPath(string directory) =>
        directory.TrimEnd('/', '\\');
}