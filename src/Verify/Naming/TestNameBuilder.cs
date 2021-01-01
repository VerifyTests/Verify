using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VerifyTests;

static class TestNameBuilder
{
    public static string ClassName(this Type type)
    {
        var name = type.Name;
        var indexOf = name.IndexOf('.');
        if (indexOf == -1)
        {
            return name;
        }

        return name.Substring(indexOf + 1, name.Length - indexOf - 1);
    }

    public static string GetUniqueTestName(MethodInfo method, IReadOnlyList<object?>? parameterValues)
    {
        string name;
        var type = method.ReflectedType!;
        if (type.IsNested)
        {
            name = $"{type.ReflectedType!.Name}.{type.Name}.{method.Name}";
        }
        else
        {
            name = $"{type.Name}.{method.Name}";
        }

        if (parameterValues == null || !parameterValues.Any())
        {
            return name;
        }

        StringBuilder builder = new();
        var parameters = method.GetParameters();
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            var parameterValue = parameterValues[index];
            builder.Append($"{parameter.Name}=");
            if (parameterValue == null)
            {
                builder.Append("null_");
                continue;
            }

            builder.Append($"{VerifierSettings.GetNameForParameter(parameterValue)}_");
        }

        builder.Length -= 1;

        return $"{name}_{builder}";
    }
}