using System;

static class Guard
{
    // ReSharper disable UnusedParameter.Global
    public static void AgainstNull(object value, string argumentName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstNullOrEmpty(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
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