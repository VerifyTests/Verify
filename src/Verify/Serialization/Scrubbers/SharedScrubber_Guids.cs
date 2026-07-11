namespace VerifyTests;

partial class Counter
{
    public bool TryConvert(Guid value, [NotNullWhen(true)] out string? result)
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

    public bool TryConvertGuid(CharSpan value, [NotNullWhen(true)] out string? result)
    {
        if (ScrubGuids)
        {
            // The shortest parseable guid (the "N" format) is 32 chars. Whitespace
            // padding, which Guid.TryParse trims, only adds length, so anything
            // shorter can never parse and the attempt can be skipped.
            if (value.Length >= 32 &&
                Guid.TryParse(value, out var guid))
            {
                result = Convert(guid);
                return true;
            }
        }

        result = null;
        return false;
    }
}