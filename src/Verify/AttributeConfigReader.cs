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

        throw new("UniqueForAssemblyConfiguration used but no `AssemblyConfigurationAttribute` found.");
    }
}