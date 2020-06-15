using System.Reflection;
using VerifyTests;

static class AttributeConfigReader
{
    public static string GetAttributeConfiguration(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        if (attribute != null)
        {
            return attribute.Configuration;
        }

        throw InnerVerifier.exceptionBuilder("UniqueForAssemblyConfiguration used but no `AssemblyConfigurationAttribute` found.");
    }
}