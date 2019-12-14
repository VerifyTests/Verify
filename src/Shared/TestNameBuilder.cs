using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

static class TestNameBuilder
{
    static string ClassName(this Type type)
    {
        var name = type.Name;
        var indexOf = name.IndexOf('.');
        if (indexOf == -1)
        {
            return name;
        }

        return name.Substring(indexOf + 1, name.Length - indexOf - 1);
    }

    public static string GetUniqueTestName(Type type, MethodInfo method, IReadOnlyList<object> parameterValues)
    {
        var name = $"{type.ClassName()}.{method.Name}";
        if (!parameterValues.Any())
        {
            return name;
        }

        var builder = new StringBuilder();
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

            builder.Append($"{parameterValue}_");
        }

        builder.Length -= 1;

        return $"{name}_{builder}";
    }
}