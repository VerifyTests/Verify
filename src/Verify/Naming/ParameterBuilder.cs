using System.Globalization;
using VerifyTests;

static class ParameterBuilder
{
    public static string Concat(Dictionary<string, object?> dictionary)
    {
        var thread = Thread.CurrentThread;
        var culture = thread.CurrentCulture;
        thread.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            return Inner(dictionary);
        }
        finally
        {
            thread.CurrentCulture = culture;
        }
    }

    public static string Inner(IReadOnlyDictionary<string, object?> dictionary)
    {
        var builder = new StringBuilder();
        foreach (var item in dictionary)
        {
            builder.Append($"{item.Key}={VerifierSettings.GetNameForParameter(item.Value)}_");
        }

        builder.Length -= 1;

        return builder.ToString();
    }
}