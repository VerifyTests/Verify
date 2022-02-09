static class ParameterBuilder
{
    public static string Concat(Dictionary<string, object?> dictionary)
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