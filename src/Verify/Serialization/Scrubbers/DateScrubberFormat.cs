namespace VerifyTests;

static class DateScrubberFormat
{
    public static bool TryGetTrimmed(string format, [NotNullWhen(true)] out string? trimmed)
    {
        if (format.EndsWith(".FFFF"))
        {
            trimmed = format[..^5];
            return true;
        }

        if (format.EndsWith(".FFF"))
        {
            trimmed = format[..^4];
            return true;
        }

        if (format.EndsWith(".FF"))
        {
            trimmed = format[..^3];
            return true;
        }

        if (format.EndsWith(".F"))
        {
            trimmed = format[..^2];
            return true;
        }

        trimmed = null;
        return false;
    }
}
