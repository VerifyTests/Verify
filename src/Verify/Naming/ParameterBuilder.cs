using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
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

        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            var value = parameterValues[index];
            builder.Append($"{parameter.Name}=");
            if (value is null)
            {
                builder.Append("null_");
                continue;
            }

            builder.Append($"{VerifierSettings.GetNameForParameter(value)}_");
        }

        builder.Length -= 1;

        return builder.ToString();
    }
}