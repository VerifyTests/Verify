using DiffEngine;

static class ClipboardEnabled
{
    static bool clipboardDisabledInEnv;

    static ClipboardEnabled()
    {
        var disabledText = Environment.GetEnvironmentVariable("Verify_DisableClipboard");
        clipboardDisabledInEnv = ParseEnvironmentVariable(disabledText);
    }

    public static bool ParseEnvironmentVariable(string? disabledText)
    {
        if (string.IsNullOrWhiteSpace(disabledText))
        {
            return false;
        }

        // Parse leniently and never throw: this runs from a static constructor,
        // so throwing would poison the type and surface as a TypeInitializationException
        // that masks the actual snapshot diff on every failing test.
        // ReSharper disable once RedundantSuppressNullableWarningExpression
        switch (disabledText!.Trim().ToLowerInvariant())
        {
            case "true":
            case "1":
            case "yes":
            case "on":
                return true;
            case "false":
            case "0":
            case "no":
            case "off":
                return false;
            default:
                Trace.WriteLine($"Could not convert `Verify_DisableClipboard` environment variable to a bool. Value: {disabledText}. Treating as not disabled.");
                return false;
        }
    }

    public static bool IsEnabled() =>
        !(
            clipboardDisabledInEnv ||
            ContinuousTestingDetector.Detected ||
            BuildServerDetector.Detected);
}
