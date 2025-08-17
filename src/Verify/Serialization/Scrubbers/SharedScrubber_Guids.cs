namespace VerifyTests;

partial class Counter
{
    internal bool TryConvert(Guid value, [NotNullWhen(true)] out string? result)
    {
        if (ScrubGuids)
        {
            result = Convert(value);
            return true;
        }

        result = null;
        return false;
    }

    internal string Convert(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            return "Guid_Empty";
        }

        return NextString(guid);
    }

    internal bool TryParseConvertGuid(CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (ScrubGuids)
        {
            if (Guid.TryParse(value, out var guid))
            {
                result = Convert(guid);
                return true;
            }
        }

        result = null;
        return false;
    }
}