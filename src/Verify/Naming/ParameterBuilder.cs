using System.Globalization;
using VerifyTests;

static class ParameterBuilder
{
    public static string Concat(MethodInfo method, IReadOnlyList<object?> parameterValues)
    {
        var parameters = method.GetParameters();

        if (parameters.Length != parameterValues.Count)
        {
            throw new($"The number of passed in parameters ({parameterValues.Count}) must match the number of parameters for the method ({parameters.Length}).");
        }

        Dictionary<string, object?> dictionary = new Dictionary<string, object?>();
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            var value = parameterValues[index];
            dictionary[parameter.Name!] = value;
        }

        return Concat(dictionary);
    }

    private static string Concat(Dictionary<string, object?> dictionary)
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