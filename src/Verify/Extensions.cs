using System.Runtime.Versioning;

static class Extensions
{
    public static string? Configuration(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        return attribute?.Configuration;
    }

    public static FrameworkName? FrameworkName(this Assembly assembly)
    {
        var targetFrameworkAttribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        if (targetFrameworkAttribute == null)
        {
            return null;
        }

        return new(targetFrameworkAttribute.FrameworkName);
    }

    public static bool IsException(this Type type)
    {
        return typeof(Exception).IsAssignableFrom(type);
    }

    public static string FullName(this MethodInfo method)
    {
        return $"{method.ReflectedType!.Name}.{method.Name}";
    }

    public static bool IsEmpty<T>(this IReadOnlyCollection<T> target)
    {
        return !target.Any();
    }

    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> valueFunc)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        return dictionary[key] = valueFunc(key);
    }
}