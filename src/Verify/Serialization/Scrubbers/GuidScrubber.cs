using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

static class GuidScrubber
{
    static readonly string GuidPattern = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b";
    static readonly Regex Regex = new(GuidPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static void ReplaceGuids(StringBuilder builder)
    {
        if (!TryReplaceGuids(builder.ToString(), SharedScrubber.Convert, out var result))
        {
            return;
        }

        builder.Clear();
        builder.Append(result);
    }

    public static bool TryReplaceGuids(string value, Func<Guid, string> guidToString, [NotNullWhen(true)] out string? result)
    {
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