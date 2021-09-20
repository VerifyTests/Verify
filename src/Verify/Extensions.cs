﻿using System.Linq;

static class Extensions
{
    public static bool IsException(this Type type)
    {
        return typeof(Exception).IsAssignableFrom(type);
    }

    public static string FullName(this MethodInfo method)
    {
        return $"{method.ReflectedType!.Name}.{method.Name}";
    }

    public static bool IsEmpty<T>(this ICollection<T> target)
    {
        return !target.Any();
    }

    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> valueFunc)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        return dictionary[key] = valueFunc(key);
    }
}