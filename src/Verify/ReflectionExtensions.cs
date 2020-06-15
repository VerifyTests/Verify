using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VerifyTests;

static class ReflectionExtensions
{
    public static List<Type> InstanceTypes(this Assembly assembly)
    {
        return assembly.GetExportedTypes()
            .Where(x => !x.IsAbstract && !x.IsNested)
            .ToList();
    }
    const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
    public static MethodInfo GetPublicMethod(this Type type, string method)
    {
        var methodInfo = type.GetMethod(method, flags);
        if (methodInfo != null)
        {
            return methodInfo;
        }
        throw InnerVerifier.exceptionBuilder($"Method `{method}` not found on type `{type.Name}`.");
    }
}