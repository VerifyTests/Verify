static class Guard
{
    public static void FileExists(string path, [CallerArgumentExpression("path")] string argumentName = "")
    {
        AgainstNullOrEmpty(path, argumentName);
        if (!File.Exists(path))
        {
            throw new ArgumentException($"File not found. Path: {path}", argumentName);
        }
    }

    public static void DirectoryExists(string path, [CallerArgumentExpression("path")] string argumentName = "")
    {
        AgainstNullOrEmpty(path, argumentName);
        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Directory not found. Path: {path}", argumentName);
        }
    }

    static char[] invalidFileChars = Path.GetInvalidFileNameChars();

    public static void BadFileNameNullable(string? name, [CallerArgumentExpression("name")] string argumentName = "")
    {
        if (name is null)
        {
            return;
        }

        BadFileName(name, argumentName);
    }

    public static void BadFileName(string name, [CallerArgumentExpression("name")] string argumentName = "")
    {
        AgainstNullOrEmpty(name, argumentName);
        foreach (var invalidChar in invalidFileChars)
        {
            if (name.IndexOf(invalidChar) == -1)
            {
                continue;
            }

            throw new ArgumentException($"Invalid character for file name. Value: {name}. Char:{invalidChar}", argumentName);
        }
    }

    static char[] invalidPathChars = Path
        .GetInvalidPathChars()
        .Concat(invalidFileChars.Except('/', '\\', ':'))
        .Distinct()
        .ToArray();

    public static void BadDirectoryName(string? name, [CallerArgumentExpression("name")] string argumentName = "")
    {
        if (name is null)
        {
            return;
        }

        AgainstEmpty(name, argumentName);
        foreach (var invalidChar in invalidPathChars)
        {
            if (name.IndexOf(invalidChar) == -1)
            {
                continue;
            }

            throw new ArgumentException($"Invalid character for directory. Value: {name}. Char:{invalidChar}", argumentName);
        }
    }

    public static void AgainstNullable(Type type, [CallerArgumentExpression("type")] string argumentName = "")
    {
        var typeFromNullable = Nullable.GetUnderlyingType(type);

        if (typeFromNullable is not null)
        {
            throw new ArgumentException("Nullable types not supported", argumentName);
        }
    }

    public static void AgainstNullOrEmpty(string value, [CallerArgumentExpression("value")] string argumentName = "")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstBadSourceFile(string sourceFile)
    {
        if (string.IsNullOrWhiteSpace(sourceFile))
        {
            throw new ArgumentNullException(
                nameof(sourceFile),
                "This can be caused by using Verify<dynamic>, which is not supported by c#. Instead call use Verify<object>.");
        }
    }

    public static void AgainstEmpty(string? value, [CallerArgumentExpression("value")] string argumentName = "")
    {
        if (value is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstNullOrEmpty(object?[] value, [CallerArgumentExpression("value")] string argumentName = "")
    {
        if (value is null)
        {
            throw new ArgumentNullException(argumentName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentNullException(argumentName, "Argument cannot be empty.");
        }
    }

    public static void AgainstNullOrEmpty<T>(T[] value, [CallerArgumentExpression("value")] string argumentName = "")
    {
        if (value is null)
        {
            throw new ArgumentNullException(argumentName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentNullException(argumentName, "Argument cannot be empty.");
        }
    }

    public static void AgainstBadExtension(string value, [CallerArgumentExpression("value")] string argumentName = "")
    {
        AgainstNullOrEmpty(value, argumentName);

        if (value.StartsWith('.'))
        {
            throw new ArgumentException("Must not start with a period ('.').", argumentName);
        }
    }
}