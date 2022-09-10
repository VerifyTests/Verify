#if !NET5_0_OR_GREATER

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter)]
    sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName) =>
            ParameterName = parameterName;

        public string ParameterName { get; }
    }
}
#endif

static class Guard
{
    public static void FileExists(string path, [CallerArgumentExpression("path")] string? paramName = null)
    {
        AgainstNullOrEmpty(path, paramName!);
        if (!File.Exists(path))
        {
            throw new ArgumentException($"File not found. Path: {path}", paramName);
        }
    }

    public static void DirectoryExists(string path, [CallerArgumentExpression("path")] string? paramName = null)
    {
        AgainstNullOrEmpty(path, paramName!);
        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Directory not found. Path: {path}", paramName);
        }
    }

    static char[] invalidFileChars = Path.GetInvalidFileNameChars();

    public static void BadFileNameNullable(string? name, [CallerArgumentExpression("name")] string? paramName = null)
    {
        if (name is null)
        {
            return;
        }

        BadFileName(name, paramName);
    }

    public static void BadFileName(string name, [CallerArgumentExpression("name")] string? paramName = null)
    {
        AgainstNullOrEmpty(name, paramName);
        foreach (var invalidChar in invalidFileChars)
        {
            if (name.IndexOf(invalidChar) == -1)
            {
                continue;
            }

            throw new ArgumentException($"Invalid character for file name. Value: {name}. Char:{invalidChar}", paramName);
        }
    }

    static char[] invalidPathChars = Path.GetInvalidPathChars()
        .Concat(invalidFileChars.Except(new[] {'/', '\\', ':'}))
        .Distinct()
        .ToArray();

    public static void BadDirectoryName(string? name, [CallerArgumentExpression("name")] string? paramName = null)
    {
        if (name is null)
        {
            return;
        }

        AgainstEmpty(name, paramName);
        foreach (var invalidChar in invalidPathChars)
        {
            if (name.IndexOf(invalidChar) == -1)
            {
                continue;
            }

            throw new ArgumentException($"Invalid character for directory. Value: {name}. Char:{invalidChar}", paramName);
        }
    }

    public static void AgainstNullable(Type type, [CallerArgumentExpression("type")] string? paramName = null)
    {
        var typeFromNullable = Nullable.GetUnderlyingType(type);

        if (typeFromNullable is not null)
        {
            throw new ArgumentException("Nullable types not supported", paramName);
        }
    }

    public static void AgainstNullOrEmpty(string value, [CallerArgumentExpression("value")] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(paramName);
        }
    }

    public static void AgainstBadSourceFile(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(
                "sourceFile",
                "This can be caused by using Verify<dynamic>, which is not supported by c#. Instead call use Verify<object>.");
        }
    }

    public static void AgainstEmpty(string? value, [CallerArgumentExpression("value")] string? paramName = null)
    {
        if (value is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(paramName);
        }
    }

    public static void AgainstNullOrEmpty(object?[] value, [CallerArgumentExpression("value")] string? paramName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentNullException(paramName, "Argument cannot be empty.");
        }
    }

    public static void AgainstNullOrEmpty<T>(T[] value, [CallerArgumentExpression("value")] string? paramName = null)
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentNullException(paramName, "Argument cannot be empty.");
        }
    }

    public static void AgainstBadExtension(string value, [CallerArgumentExpression("value")] string? paramName = null)
    {
        AgainstNullOrEmpty(value, paramName);

        if (value.StartsWith("."))
        {
            throw new ArgumentException("Must not start with a period ('.').", paramName);
        }
    }
}