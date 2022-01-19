using System.Diagnostics.CodeAnalysis;

namespace VerifyTests;

public partial class SerializationSettings
{
    internal bool TryConvertString(string value, [NotNullWhen(true)] out string? result)
    {
        if (TryParseConvertGuid(value, out result))
        {
            return true;
        }

        if (TryParseConvertDateTimeOffset(value, out result))
        {
            return true;
        }

        if (TryParseConvertDateTime(value, out result))
        {
            return true;
        }

#if NET6_0_OR_GREATER
        if (TryParseConvertDate(value, out result))
        {
            return true;
        }
#endif

        result = null;
        return false;
    }
}