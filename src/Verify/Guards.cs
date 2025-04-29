static class Guards
{
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
        Guard.NotNullOrEmpty(name, argumentName);
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
        .Concat(invalidFileChars.Except(['/', '\\', ':']))
        .Distinct()
        .ToArray();

    public static void BadDirectoryName(string? name, [CallerArgumentExpression("name")] string argumentName = "")
    {
        if (name is null)
        {
            return;
        }

        Guard.NotEmpty(name, argumentName);
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

    public static void AgainstBadSourceFile(string sourceFile)
    {
        if (string.IsNullOrWhiteSpace(sourceFile))
        {
            Debugger.Launch();
            throw new ArgumentNullException(
                nameof(sourceFile),
                "This can be caused by using Verify<dynamic>, which is not supported by c#. Instead call use Verify<object>.");
        }
    }

    public static void AgainstBadExtension(string value, [CallerArgumentExpression("value")] string argumentName = "")
    {
        Guard.NotNullOrEmpty(value, argumentName);

        if (value.StartsWith('.'))
        {
            throw new ArgumentException("Must not start with a period ('.').", argumentName);
        }

        if (value.Contains('\\') || value.Contains('/'))
        {
            throw new ArgumentException("Must not contain a directory separator.", argumentName);
        }
    }
}