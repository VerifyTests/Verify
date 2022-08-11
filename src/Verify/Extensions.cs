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

    public static string FixNewlines(this string value) =>
        value.Replace("\r\n", "\n")
            .Replace('\r', '\n');

    public static char? FirstChar(this StringBuilder builder)
    {
        if (builder.Length > 0)
        {
            return builder[0];
        }

        return null;
    }

    public static char? LastChar(this StringBuilder builder)
    {
        if (builder.Length > 0)
        {
            return builder[^1];
        }

        return null;
    }

    public static void RemoveEmptyLines(this StringBuilder builder)
    {
        builder.FilterLines(string.IsNullOrWhiteSpace);
        if (builder.FirstChar() is '\n')
        {
            builder.Remove(0, 1);
        }

        if (builder.LastChar() is '\n')
        {
            builder.Length--;
        }
    }

    public static void ReplaceIfLonger(this StringBuilder builder, string oldValue, string newValue)
    {
        if (builder.Length < oldValue.Length)
        {
            return;
        }

        builder.Replace(oldValue, newValue);
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

    public static bool EndsWith(this string value, char ch)
    {
        var lastPos = value.Length - 1;
        if (lastPos >= value.Length)
        {
            return false;
        }

        return value[lastPos] == ch;
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