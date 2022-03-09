namespace VerifyTests;

public partial class SerializationSettings
{
    internal bool TryConvert(Counter counter, Guid value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubGuids)
        {
            result = null;
            return false;
        }

        result = Convert(counter, value);
        return true;
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
}