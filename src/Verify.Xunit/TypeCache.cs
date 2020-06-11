using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Verify;
using VerifyXunit;

static class TypeCache
{
    static List<Type> types = null!;


    const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

    public static MethodInfo GetInfo(string file, string method)
    {
        if (InjectInfoAttribute.TryGet(out var info))
        {
            return info!;
        }

        if (types == null)
        {
            if (SharedVerifySettings.assembly == null)
            {
                throw new Exception("Call `SharedVerifySettings.SetTestAssembly(Assembly.GetExecutingAssembly());` at assembly startup. Or, alternatively, a `[InjectInfo]` to the type.");
            }

            types = SharedVerifySettings.assembly.GetExportedTypes()
                .Where(x => !x.IsAbstract && !x.IsNested)
                .ToList();
        }

        var type = FindType(file);

        var methodInfo = type.GetMethod(method, flags);
        if (methodInfo == null)
        {
            throw new Exception($"Method `{method}` not found on type `{type.Name}`. File: {file}");
        }

        return methodInfo;
    }

    static Type FindType(string file)
    {
        var withoutExtension = file.Substring(0, file.LastIndexOf('.'));
        var withDots = withoutExtension
            .Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.');
        foreach (var type in types)
        {
            if (withDots.EndsWith($".{type.FullName}"))
            {
                return type;
            }
        }
        foreach (var type in types)
        {
            if (withDots.EndsWith($".{type.Name}"))
            {
                return type;
            }
        }

        throw new Exception($"Unable to find type for file `{file}`. There are some known scenarios where the types cannot be derived (for example partial or nested classes). In these case add a `[InjectInfo]` to the type.");
    }
}