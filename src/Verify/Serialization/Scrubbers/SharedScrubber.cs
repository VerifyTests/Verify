partial class SerializationSettings
{
    internal bool TryConvertString(Counter counter, CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (TryParseConvertGuid(counter, value, out result) ||
            TryParseConvertDateTimeOffset(counter, value, out result)||
            TryParseConvertDateTime(counter, value, out result))
        {
            return true;
        }

#if NET6_0_OR_GREATER
        if (TryParseConvertDate(counter, value, out result) ||
            TryParseConvertTime(counter, value, out result))
        {
            return true;
        }
#endif

        result = null;
        return false;
    }
}