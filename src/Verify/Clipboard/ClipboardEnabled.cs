using VerifyTests;

static class ClipboardEnabled
{
    static bool clipboardDisabled;

    static ClipboardEnabled()
    {
        var disabledText = EnvironmentEx.GetEnvironmentVariable("Verify_DisableClipboard");
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

        throw InnerVerifier.exceptionBuilder($"Could not convert `Verify_DisableClipboard` environment variable to a bool. Value: {disabledText}");
    }

    public static bool IsEnabled(VerifySettings settings)
    {
        if (clipboardDisabled)
        {
            return false;
        }

        if (settings.clipboardEnabled == null)
        {
            return !VerifierSettings.clipboardDisabled;
        }

        return settings.clipboardEnabled.Value;
    }
}