using System;
using System.Collections.Generic;

static class Extensions
{
    public static bool IsException(this Type type)
    {
        return typeof(Exception).IsAssignableFrom(type);
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