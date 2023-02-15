static class Extensions
{
    public static string Extension(this FileStream file) =>
        FileExtensions.GetExtension(file.Name);

    public static async Task<List<T>> ToList<T>(this IAsyncEnumerable<T> target)
    {
        var list = new List<T>();
        await foreach (var item in target)
        {
            list.Add(item);
        }

        return list;
    }

    public static string TrimPreamble(this string text) =>
        text.TrimStart('\uFEFF');

    public static void Enqueue<T>(this Queue<T> queue, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            queue.Enqueue(item);
        }
    }

    public static List<T> Clone<T>(this List<T> original) =>
        new(original);

    public static IReadOnlyList<string>? ParameterNames(this MethodInfo method)
    {
        var parameters = method.GetParameters();
        if (parameters.Length == 0)
        {
            return null;
        }

        return parameters
            .Select(_ => _.Name!)
            .ToList();
    }

    public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> original)
        where TValue : struct where TKey : notnull => new(original);

    public static Dictionary<TKey, TValue?> Clone<TKey, TValue>(this Dictionary<TKey, TValue?> original)
        where TValue : struct where TKey : notnull => new(original);

    public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType = StringComparison.CurrentCulture)
    {
        if (input.EndsWith(suffixToRemove, comparisonType))
        {
            return input[..^suffixToRemove.Length];
        }

        return input;
    }

    #region NameWithParent

    public static string NameWithParent(this Type type)
    {
        if (type.IsNested)
        {
            return $"{type.ReflectedType!.Name}.{type.Name}";
        }

        return type.Name;
    }

    #endregion

    public static string? Configuration(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>();
        return attribute?.Configuration;
    }

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

    public static string Remove(this string value, string toRemove) =>
        value.Replace(toRemove, "");

    public static string RemoveLast(this string value, string pattern)
    {
        var place = value.LastIndexOf(pattern, StringComparison.OrdinalIgnoreCase);

        if (place == -1)
        {
            return value;
        }

        return value.Remove(place, pattern.Length);
    }

    public static void ReplaceIfLonger(this StringBuilder builder, string oldValue, string newValue)
    {
        if (builder.Length < oldValue.Length)
        {
            return;
        }

        builder.Replace(oldValue, newValue);
    }

#if NET472 || NET462 || NET48 || NETSTANDARD2_0
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
        var attribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        if (attribute is null)
        {
            return null;
        }

        return new(attribute.FrameworkName);
    }

    public static bool IsException(this Type type) =>
        typeof(Exception).IsAssignableFrom(type);
}