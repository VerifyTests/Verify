using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

static class ReflectionExtensions
{
    public static List<Type> InstanceTypes(this Assembly assembly)
    {
        return assembly.GetExportedTypes()
            .Where(x => !x.IsAbstract && !x.IsNested)
            .ToList();
    }
}