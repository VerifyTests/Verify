// ReSharper disable UnusedVariable

static class Extensions
{
    public static string Extension(this FileStream file) =>
        Path.GetExtension(file.Name)[1..];

    public static bool ContainsNewline(this CharSpan span) =>
        span.IndexOfAny('\r', '\n') != -1;

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
        catch (NotSupportedException)
        {
            return false;
        }

        return true;
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

    public static CharSpan AsSpan(this StringBuilder builder) =>
        builder
            .ToString()
            .AsSpan();

    public static List<T> Clone<T>(this List<T> original) =>
        [..original];

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
        where TValue : struct
        where TKey : notnull => new(original);

    public static Dictionary<TKey, TValue?> Clone<TKey, TValue>(this Dictionary<TKey, TValue?> original)
        where TValue : struct
        where TKey : notnull => new(original);

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

    public static void FilterLines(this StringBuilder input, Func<string, bool> removeLine)
    {
        var theString = input.ToString();
        using var reader = new StringReader(theString);
        input.Clear();

        while (reader.ReadLine() is { } line)
        {
            if (removeLine(line))
            {
                continue;
            }

            input.AppendLineN(line);
        }

        if (input.Length > 0 &&
            !theString.EndsWith('\n'))
        {
            input.Length -= 1;
        }
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

    public static void Overwrite(this StringBuilder builder, string value, int index, int length)
    {
        builder.Remove(index, length);
        builder.Insert(index, value);
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

    public static bool IsException(this Type type) =>
        typeof(Exception).IsAssignableFrom(type);
}