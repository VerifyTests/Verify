class DictionaryWrapper<TKey, TValue, TInner> :
    Dictionary<TKey, TValue>,
    IDictionaryWrapper
    where TKey : notnull
    where TInner : IDictionary<TKey, TValue>
{
    public DictionaryWrapper(TInner inner) :
        base(inner.ToDictionary(x => x.Key, x => x.Value))
    {
    }
}