using System;
using DiffEngine;
using VerifyTests;

static class ClipboardEnabled
{
    static bool clipboardDisabled;

    static ClipboardEnabled()
    {
        var disabledText = Environment.GetEnvironmentVariable("Verify_DisableClipboard");
        clipboardDisabled = ParseEnvironmentVariable(disabledText);
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
        if (clipboardDisabled)
        {
            return false;
        }

        if (BuildServerDetector.Detected)
        {
            return false;
        }

        if (ContinuousTestingDetector.Detected)
        {
            return false;
        }

        return !VerifierSettings.clipboardDisabled;
    }
}