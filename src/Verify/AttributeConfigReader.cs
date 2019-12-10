using System;
using System.Reflection;

static class AttributeConfigReader
{
    public static string GetAttributeConfiguration(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        if (attribute != null)
        {
            return attribute.Configuration;
        }

        throw new Exception("UniqueForAssemblyConfiguration used but no `AssemblyConfigurationAttribute` found.");
    }
}