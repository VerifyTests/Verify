﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VerifyTests;

static class TestPrefixBuilder
{
    public static string GetPrefix(Type type, MethodInfo method, IReadOnlyList<object?>? parameterValues)
    {
        string name;
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