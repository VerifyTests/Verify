namespace VerifyTests;

public partial class Counter
{
    internal bool TryParseConvert(CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (TryParseConvertGuid(value, out result) ||
            TryParseConvertDateTimeOffset(value, out result) ||
            TryParseConvertDateTime(value, out result))
        {
            return true;
        }

#if NET6_0_OR_GREATER
        if (TryParseConvertDate(value, out result) ||
            TryParseConvertTime(value, out result))
        {
            return true;
        }
#endif

        result = null;
        return false;
    }

    internal bool TryConvertString(CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (TryParseConvertGuid(value, out result) ||
            TryParseConvertDateTimeOffset(value, out result)||
            TryParseConvertDateTime(value, out result))
        {
            return true;
        }

#if NET6_0_OR_GREATER
        if (TryParseConvertDate(value, out result) ||
            TryParseConvertTime(value, out result))
        {
            return true;
        }
#endif

        result = null;
        return false;
    }
}