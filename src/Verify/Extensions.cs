// ReSharper disable UnusedVariable
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

    // Streams can throw for Length. Eg a http stream that the server has not specified the length header
    // Specify buffer to avoid an exception in Stream.CopyToAsync where it reads Length
    // https://github.com/dotnet/runtime/issues/43448
    public static Task SafeCopy(this Stream source, Stream target)
    {
        if (source.CanReadLength())
        {
            return source.CopyToAsync(target);
        }

        return source.CopyToAsync(target, 81920);
    }

    public static bool CanSeekAndReadLength(this Stream stream) =>
        stream.CanSeek &&
        CanReadLength(stream);

    static bool CanReadLength(this Stream stream)
    {
        try
        {
            var streamLength = stream.Length;
        }
        catch (NotImplementedException)
        {
            return false;
        }

        return true;
    }

    public static string TrimPreamble(this string text) =>
        text.TrimStart('\uFEFF');

    public static bool Contains(this StringBuilder builder, char ch)
    {
        for (var index = 0; index < builder.Length; index++)
        {
            var item = builder[index];
            if (ch == item)
            {
                return true;
            }
        }

        return false;
    }

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

    public static int Count(this StringBuilder builder, char ch)
    {
        var count = 0;
        for (var index = 0; index < builder.Length; index++)
        {
            if (builder[index] == ch)
            {
                count++;
            }
        }

        return count;
    }

    public static FrameworkNameVersion? FrameworkName(this Assembly assembly)
    {
        var attribute = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
        if (attribute is null)
        {
            return null;
        }

        var frameworkName = new FrameworkName(attribute.FrameworkName);
        var name = Namer.GetSimpleFrameworkName(frameworkName);
        var version = frameworkName.Version;
        return new (name, $"{name}{version.Major}_{version.Minor}");
    }

    public static bool IsException(this Type type) =>
        typeof(Exception).IsAssignableFrom(type);
}