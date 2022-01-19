using System.Diagnostics.CodeAnalysis;

partial class SharedScrubber
{
    internal bool TryConvert(Guid value, [NotNullWhen(true)] out string? result)
    {
        if (!serializationSettings.scrubGuids)
        {
            result = null;
            return false;
        }

        result = Convert(value);
        return true;
    }

    internal static string Convert(Guid guid)
    {
        if (guid == Guid.Empty)
        {
            return "Guid_Empty";
        }

        var next = CounterContext.Current.Next(guid);
        return $"Guid_{next}";
    }

    internal bool TryParseConvertGuid(string value, [NotNullWhen(true)] out string? result)
    {
        if (serializationSettings.scrubGuids)
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