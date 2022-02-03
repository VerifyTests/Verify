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
        if (disabledText is null)
        {
            return false;
        }

        if (bool.TryParse(disabledText, out var disabled))
        {
            return disabled;
        }

        throw new($"Could not convert `Verify_DisableClipboard` environment variable to a bool. Value: {disabledText}");
    }

    public static bool IsEnabled()
    {
        return !(
            clipboardDisabledInEnv ||
            ContinuousTestingDetector.Detected ||
            BuildServerDetector.Detected);
    }
}