using System.Collections.Concurrent;
using System.Threading;

class Counter<T>
    where T : struct
{
    ConcurrentDictionary<T, int> cache = new();
    int current;

    public int IntOrNext(T input)
    {
        return cache.GetOrAdd(input, _ => Interlocked.Increment(ref current));
    }
}