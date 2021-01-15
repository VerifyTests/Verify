using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VerifyTests;

static class ParameterBuilder
{
    public static string Concat(MethodInfo method, IReadOnlyList<object?> parameterValues)
    {
        StringBuilder builder = new();
        var parameters = method.GetParameters();
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            var value = parameterValues[index];
            builder.Append($"{parameter.Name}=");
            if (value == null)
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