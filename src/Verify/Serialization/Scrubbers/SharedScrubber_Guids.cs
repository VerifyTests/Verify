using System.Diagnostics.CodeAnalysis;

partial class SharedScrubber
{
    bool scrubGuids;

    public bool TryConvert(Guid value, [NotNullWhen(true)] out string? result)
    {
        if (!scrubGuids)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

    public static string Convert(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            return "Guid_Empty";
        }

        var next = CounterContext.Current.Next(guid);
        return $"Guid_{next}";
    }

    public bool TryParseConvertGuid(string value, [NotNullWhen(true)] out string? result)
    {
        if (scrubGuids)
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