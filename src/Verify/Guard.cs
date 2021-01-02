using System;
using System.IO;
using System.Linq;

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

    public static void EnsurePathFullyQualified(string path)
    {
        if (!path.Any(x =>
            x == Path.AltDirectorySeparatorChar ||
            x == Path.DirectorySeparatorChar))
        {
            throw new Exception($"Path must be fully qualified. Path: {path}");
        }
    }

    public static void FileExists(string path, string argumentName)
    {
        AgainstNullOrEmpty(argumentName, path);
        if (!File.Exists(path))
        {
            throw new ArgumentException($"File not found. Path: {path}");
        }
    }

    public static void DirectoryExists(string path, string argumentName)
    {
        AgainstNullOrEmpty(argumentName, path);
        if (!Directory.Exists(path))
        {
            throw new ArgumentException($"Directory not found. Path: {path}");
        }
    }

    public static void AgainstNullOrEmpty(string value, string argumentName)
    {
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