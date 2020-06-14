using System;
using VerifyTesting;

static class ClipboardEnabled
{
    static bool clipboardDisabled;

    static ClipboardEnabled()
    {
        var disabledText = Environment.GetEnvironmentVariable("Verify.DisableClipboard");
        clipboardDisabled = ParseEnvironmentVariable(disabledText);
    }

    public static bool ParseEnvironmentVariable(string? disabledText)
    {
        if (disabledText == null)
        {
            return false;
        }

        if (bool.TryParse(disabledText, out var disabled))
        {
            return disabled;
        }

        throw new Exception($"Could not convert `Verify.DisableClipboard` environment variable to a bool. Value: {disabledText}");
    }

    public static bool IsEnabled(VerifySettings settings)
    {
        if (clipboardDisabled)
        {
            return false;
        }

        if (settings.clipboardEnabled == null)
        {
            return !SharedVerifySettings.clipboardDisabled;
        }

        return settings.clipboardEnabled.Value;
    }
}