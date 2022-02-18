namespace VerifyTests;

public partial class SerializationSettings
{
    internal bool TryConvertString(Counter counter, string value, [NotNullWhen(true)] out string? result)
    {
        if (TryParseConvertGuid(counter, value, out result))
        {
            return true;
        }

        if (TryParseConvertDateTimeOffset(counter, value, out result))
        {
            return true;
        }

        if (TryParseConvertDateTime(counter, value, out result))
        {
            return true;
        }

#if NET6_0_OR_GREATER
        if (TryParseConvertDate(counter, value, out result))
        {
            return true;
        }
#endif

        result = null;
        return false;
    }
}