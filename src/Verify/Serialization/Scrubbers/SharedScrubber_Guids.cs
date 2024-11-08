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

    internal bool TryParseConvertGuid(Counter counter, CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (scrubGuids)
        {
            if (GuidPolyfill.TryParse(value, out var guid))
            {
                result = Convert(counter, guid);
                return true;
            }
        }

        result = null;
        return false;
    }
}