using System.Globalization;
using VerifyTests;

static class ParameterBuilder
{
    public static string Concat(MethodInfo method, IReadOnlyList<object?> parameterValues)
    {
        var thread = Thread.CurrentThread;
        var culture = thread.CurrentCulture;
        thread.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            return Inner(method, parameterValues);
        }
        finally
        {
            thread.CurrentCulture = culture;
        }
    }

    static string Inner(MethodInfo method, IReadOnlyList<object?> parameterValues)
    {
        var builder = new StringBuilder();
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

        foreach (var item in dictionary)
        {
            builder.Append($"{item.Key}={VerifierSettings.GetNameForParameter(item.Value)}_");
        }

        builder.Length -= 1;

        return builder.ToString();
    }
}