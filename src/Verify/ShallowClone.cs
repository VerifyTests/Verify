using System.Collections.Generic;

static class ShallowClone
{
    public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> original)
        where TKey : class
    {
        var ret = new Dictionary<TKey, TValue>(original.Count, original.Comparer);
        foreach (var entry in original)
        {
            ret.Add(entry.Key, entry.Value);
        }

        return ret;
    }

    public static List<T> Clone<T>(this List<T> original)
    {
        return new(original);
    }
}