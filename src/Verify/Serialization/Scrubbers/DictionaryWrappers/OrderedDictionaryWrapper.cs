class OrderedDictionaryWrapper<TKey, TValue, TInner> :
    Dictionary<TKey, TValue>,
    IDictionaryWrapper
    where TKey : notnull
    where TInner : IDictionary<TKey, TValue>
{
    static Comparer<TKey> compare = Comparer<TKey>.Default;
    static bool isComparable = typeof(IComparable).IsAssignableFrom(typeof(TKey));

    public OrderedDictionaryWrapper(TInner inner) :
        base(Wrap(inner))
    {
    }

    static IDictionary<TKey, TValue> Wrap(TInner inner)
    {
        if (!isComparable)
        {
            return inner;
        }

        return inner
            .OrderBy(pair => pair.Key, compare)
            .ToDictionary(x => x.Key, x => x.Value);
    }
}