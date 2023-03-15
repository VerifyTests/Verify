static class Extensions
{
    public static string Extension(this FileStream file) =>
        FileExtensions.GetExtension(file.Name);

    public static Version MajorMinor(this Version version) =>
        new(version.Major, version.Minor);

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

#if NET462 || NET472 || NET48 || NETSTANDARD2_0

    public static bool SequenceEqual(this CharSpan value1, string value2)
    {
        if (value1.Length != value2.Length)
        {
            return false;
        }

        for (var index = 0; index < value1.Length; index++)
        {
            var ch1 = value1[index];
            var ch2 = value2[index];
            if (ch1 != ch2)
            {
                return false;
            }
        }

        return true;
    }

#endif

    public static FrameworkNameVersion? FrameworkName(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        if (attribute is null)
        {
            return null;
        }

        var frameworkName = new FrameworkName(attribute.FrameworkName);
        var name = Namer.GetSimpleFrameworkName(frameworkName);
        return new (name, $"{name}{frameworkName.Version.Major}_{frameworkName.Version.Minor}");
    }

    public static bool IsException(this Type type) =>
        typeof(Exception).IsAssignableFrom(type);
}