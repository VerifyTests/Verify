using System.Runtime.Versioning;

static class Extensions
{
    public static List<T> Clone<T>(this List<T> original) =>
        new(original);

    public static string? Configuration(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        return attribute?.Configuration;
    }

#if NET472 || NET461  || NET48 || NETSTANDARD2_0
    public static bool StartsWith(this string value, char ch)
    {
        if (value.Length == 0)
        {
            return false;
        }

        return value[0] == ch;
    }
#endif

    public static FrameworkName? FrameworkName(this Assembly assembly)
    {
        var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        if (targetFrameworkAttribute == null)
        {
            return null;
        }

        return new(targetFrameworkAttribute.FrameworkName);
    }

    public static bool IsException(this Type type) =>
        typeof(Exception).IsAssignableFrom(type);

    public static bool IsEmpty<T>(this IReadOnlyCollection<T> target) =>
        !target.Any();
}