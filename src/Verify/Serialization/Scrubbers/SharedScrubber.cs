using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

partial class SharedScrubber
{
    JsonSerializerSettings settings;

    public SharedScrubber(bool scrubGuids, bool scrubDateTimes, JsonSerializerSettings settings)
    {
        this.scrubGuids = scrubGuids;
        this.scrubDateTimes = scrubDateTimes;
        this.settings = settings;
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