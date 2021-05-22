using System;
using System.IO;

static class Guard
{
    // ReSharper disable UnusedParameter.Global
    public static void AgainstNull(object? value, string argumentName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void FileExists(string path, string argumentName)
    {
        AgainstNullOrEmpty(path, argumentName);
        if (!File.Exists(path))
        {
            throw new ArgumentException($"File not found. Path: {path}", argumentName);
        }
    }

    static char[] invalidFileChars = Path.GetInvalidFileNameChars();

    public static void BadFileNameNullable(string? name, string argumentName)
    {
        if (name == null)
        {
            return;
        }

        BadFileName(name, argumentName);
    }

    public static void BadFileName(string name, string argumentName)
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

    static char[] invalidPathChars = Path.GetInvalidPathChars();

    public static void BadDirectoryName(string? name, string argumentName)
    {
        if (name == null)
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

    public static void AgainstNullOrEmpty(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstEmpty(string? value, string argumentName)
    {
        if (value == null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstNullOrEmpty(object?[] value, string argumentName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentNullException(argumentName, "Argument cannot be empty.");
        }
    }

    public static void AgainstNullOrEmpty<T>(T[] value, string argumentName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }

        if (value.Length == 0)
        {
            throw new ArgumentNullException(argumentName, "Argument cannot be empty.");
        }
    }

    public static void AgainstBadExtension(string value, string argumentName)
    {
        AgainstNullOrEmpty(value, argumentName);

        if (value.StartsWith("."))
        {
            throw new ArgumentException("Must not start with a period ('.').", argumentName);
        }
    }
}