using System;
using Verify;

static class ClipboardEnabled
{
    static bool clipboardDisabled;

    static ClipboardEnabled()
    {
        var disabledText = Environment.GetEnvironmentVariable("Verify.DisableClipboard");
        if (disabledText == null)
        {
            return;
        }

        if (bool.TryParse(disabledText, out clipboardDisabled))
        {
            return;
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