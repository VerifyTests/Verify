using System.Diagnostics.CodeAnalysis;
using VerifyTests;

partial class SharedScrubber
{
    SerializationSettings serializationSettings;

    public SharedScrubber(SerializationSettings serializationSettings)
    {
        this.serializationSettings = serializationSettings;
    }

    public bool TryConvertString(string value, [NotNullWhen(true)] out string? result)
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