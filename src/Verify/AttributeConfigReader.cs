using System.Reflection;

static class AttributeConfigReader
{
    public static string GetAttributeConfiguration(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        if (attribute is not null)
        {
            return attribute.Configuration;
        }

        throw new("UniqueForAssemblyConfiguration used but no `AssemblyConfigurationAttribute` found.");
    }
}