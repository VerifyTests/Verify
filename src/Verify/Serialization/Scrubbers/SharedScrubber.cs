namespace VerifyTests;

public partial class Counter
{
    public bool TryConvert(CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (TryConvertGuid(value, out result) ||
            TryConvertDateTimeOffset(value, out result) ||
            TryConvertDateTime(value, out result) ||
            TryConvertDate(value, out result))
        {
            return true;
        }

#if NET6_0_OR_GREATER
        if (TryConvertTime(value, out result))
        {
            return true;
        }
#endif

        result = null;
        return false;
    }
}