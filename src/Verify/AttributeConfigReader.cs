static class AttributeConfigReader
{
    public static string? GetAttributeConfiguration(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        return attribute?.Configuration;
    }
}