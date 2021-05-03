using System.Collections.Concurrent;
using System.Threading;

class Counter<T>
    where T : struct
{
    ConcurrentDictionary<T, int> cache = new();
    int current;

    public int IntOrNext(T input)
    {
        if (cache.TryGetValue(input, out var cached))
        {
            return cached;
        }

        var increment = Interlocked.Increment(ref current);
        cache[input] = increment;
        return increment;
    }
}