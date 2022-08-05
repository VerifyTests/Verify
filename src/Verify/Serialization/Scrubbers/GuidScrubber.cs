static class GuidScrubber
{
    static readonly string GuidPattern = @"\{[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}\}";
    static readonly Regex Regex = new(GuidPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static void ReplaceGuids(StringBuilder builder)
    {
        if (!TryReplaceGuids(
                builder.ToString(),
                guid => SerializationSettings.Convert(Counter.Current, guid),
                out var result))
        {
            return;
        }

        builder.Clear();
        builder.Append(result);
    }

    static bool TryReplaceGuids(string value, Func<Guid, string> guidToString, [NotNullWhen(true)] out string? result)
    {
        if (Guid.TryParseExact(value, "D", out var fullGuid))
        {
            result = guidToString(fullGuid);
            return true;
        }

        var guids = Regex.Matches(value);
        if (guids.Count > 0)
        {
            result = value;
            foreach (Match? id in guids)
            {
                var stringGuid = id!.Value;
                var guid = Guid.Parse(stringGuid);
                var convertedGuid = guidToString(guid);

                result = result.Replace(stringGuid, convertedGuid);
            }

            return true;
        }

        result = null;
        return false;
    }
}