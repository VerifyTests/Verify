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
    public static void FileExists(string path, [CallerArgumentExpression("path")] string parameterName = "")
    {
        AgainstNullOrEmpty(path, parameterName);
        if (!File.Exists(path))
        {
            throw new ArgumentException($"File not found. Path: {path}", parameterName);
        }
    }

    public static void DirectoryExists(string path, [CallerArgumentExpression("path")] string parameterName = "")
    {
        AgainstNullOrEmpty(path, parameterName);
        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Directory not found. Path: {path}", parameterName);
        }
    }

    static char[] invalidFileChars = Path.GetInvalidFileNameChars();

    public static void BadFileNameNullable(string? name, [CallerArgumentExpression("name")] string parameterName = "")
    {
        if (name is null)
        {
            return;
        }

        BadFileName(name, parameterName);
    }

    public static void BadFileName(string name, [CallerArgumentExpression("name")] string parameterName = "")
    {
        AgainstNullOrEmpty(name, parameterName);
        foreach (var invalidChar in invalidFileChars)
        {
            if (name.IndexOf(invalidChar) == -1)
            {
                continue;
            }

            throw new ArgumentException($"Invalid character for file name. Value: {name}. Char:{invalidChar}", parameterName);
        }
    }

    static char[] invalidPathChars = Path.GetInvalidPathChars()
        .Concat(invalidFileChars.Except(new[] {'/', '\\', ':'}))
        .Distinct()
        .ToArray();

    public static void BadDirectoryName(string? name, [CallerArgumentExpression("name")] string parameterName = "")
    {
        if (name is null)
        {
            return;
        }

        AgainstEmpty(name, parameterName);
        foreach (var invalidChar in invalidPathChars)
        {
            if (name.IndexOf(invalidChar) == -1)
            {
                continue;
            }

            throw new ArgumentException($"Invalid character for directory. Value: {name}. Char:{invalidChar}", parameterName);
        }
    }

    public static void AgainstNullable(Type type, [CallerArgumentExpression("type")] string parameterName = "")
    {
        var typeFromNullable = Nullable.GetUnderlyingType(type);

        if (typeFromNullable is not null)
        {
            throw new ArgumentException("Nullable types not supported", parameterName);
        }
    }

    public static void AgainstNullOrEmpty(string value, [CallerArgumentExpression("value")] string parameterName = "")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(parameterName);
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

    public static void AgainstEmpty(string? value, [CallerArgumentExpression("value")] string parameterName = "")
    {
        if (value is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(parameterName);
        }
    }

    public static void AgainstNullOrEmpty(object?[] value, [CallerArgumentExpression("value")] string parameterName = "")
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentNullException(parameterName, "Argument cannot be empty.");
        }
    }

    public static void AgainstNullOrEmpty<T>(T[] value, [CallerArgumentExpression("value")] string parameterName = "")
    {
        if (value is null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentNullException(parameterName, "Argument cannot be empty.");
        }
    }

    public static void AgainstBadExtension(string value, [CallerArgumentExpression("value")] string parameterName = "")
    {
        AgainstNullOrEmpty(value, parameterName);

        if (value.StartsWith("."))
        {
            throw new ArgumentException("Must not start with a period ('.').", parameterName);
        }
    }
}