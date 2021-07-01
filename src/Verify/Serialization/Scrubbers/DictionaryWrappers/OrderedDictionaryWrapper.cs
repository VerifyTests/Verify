using System.Collections.Generic;
using System.Linq;

class OrderedDictionaryWrapper<TKey, TValue, TInner> :
    Dictionary<TKey, TValue>,
    IDictionaryWrapper
    where TKey : notnull
    where TInner : IDictionary<TKey, TValue>
{
    static Comparer<TKey> compare = Comparer<TKey>.Default;

    public OrderedDictionaryWrapper(TInner inner) :
        base(inner
            .OrderBy(pair => pair.Key, compare)
            .ToDictionary(x => x.Key, x => x.Value))
    {
    }
}