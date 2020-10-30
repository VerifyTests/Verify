using System;

static class ReflectionExtensions
{
    public static bool IsException(this Type type)
    {
        return typeof(Exception).IsAssignableFrom(type);
    }
}