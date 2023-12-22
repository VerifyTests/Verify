partial class SerializationSettings
{
    internal bool TryConvert(Counter counter, Guid value, [NotNullWhen(true)] out string? result)
    {
        if (scrubGuids)
        {
            result = Convert(counter, value);
            return true;
        }

        result = null;
        return false;
    }

    internal static string Convert(Counter counter, Guid guid)
    {
        if (guid == Guid.Empty)
        {
            return "Guid_Empty";
        }

        return counter.NextString(guid);
    }

    internal bool TryParseConvertGuid(Counter counter, string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubGuids)
        {
            if (Guid.TryParse(value, out var guid))
            {
                result = Convert(counter, guid);
                return true;
            }
        }

        result = null;
        return false;
    }

    internal bool TryParseConvert(Counter counter, string value, [NotNullWhen(true)] out string? result)
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

        if (TryParseConvertTime(counter, value, out result))
        {
            return true;
        }
#endif

        result = null;
        return false;
    }
}