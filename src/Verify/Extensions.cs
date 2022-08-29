static class Extensions
{
    public static List<T> Clone<T>(this List<T> original) =>
        new(original);

    public static List<string> MethodNames(this MethodInfo method) =>
        method.GetParameters()
            .Select(_ => _.Name!)
            .ToList();

    public static Dictionary<TKey, TValue>  Clone<TKey, TValue>(this Dictionary<TKey, TValue> original)
        where TValue : struct where TKey : notnull => new(original);

    public static Dictionary<TKey, TValue?>  Clone<TKey, TValue>(this Dictionary<TKey, TValue?> original)
        where TValue : struct where TKey : notnull => new(original);

    public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (input.EndsWith(suffixToRemove, comparisonType))
        {
            return input[..^suffixToRemove.Length];
        }

        return input;
    }

    public static string? Configuration(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        return attribute?.Configuration;
    }

    public static string FixNewlines(this string value) =>
        value
            .Replace("\r\n", "\n")
            .Replace('\r', '\n');

    public static char? FirstChar(this StringBuilder builder)
    {
        if (builder.Length > 0)
        {
            return builder[0];
        }

        return null;
    }

    static char[] invalidPathChars =
    {
        '"',
        '\\',
        '<',
        '>',
        '|',
        '\u0000',
        '\u0001',
        '\u0002',
        '\u0003',
        '\u0004',
        '\u0005',
        '\u0006',
        '\u0007',
        '\b',
        '\t',
        '\n',
        '\u000b',
        '\f',
        '\r',
        '\u000e',
        '\u000f',
        '\u0010',
        '\u0011',
        '\u0012',
        '\u0013',
        '\u0014',
        '\u0015',
        '\u0016',
        '\u0017',
        '\u0018',
        '\u0019',
        '\u001a',
        '\u001b',
        '\u001c',
        '\u001d',
        '\u001e',
        '\u001f',
        ':',
        '*',
        '?',
        '/'
    };

    public static string ReplaceInvalidPathChars(this string value)
    {
        var builder = new StringBuilder();
        foreach (var ch in value)
        {
            if (invalidPathChars.Contains(ch))
            {
                builder.Append('-');
            }
            else
            {
                builder.Append(ch);
            }
        }

        return builder.ToString();
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

    public static string Remove(this string value, string toRemove) =>
        value.Replace(toRemove, "");

    public static void ReplaceIfLonger(this StringBuilder builder, string oldValue, string newValue)
    {
        if (builder.Length < oldValue.Length)
        {
            return;
        }

        builder.Replace(oldValue, newValue);
    }

#if NET472 || NET461 || NET48 || NETSTANDARD2_0
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
        if (targetFrameworkAttribute is null)
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