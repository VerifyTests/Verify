class DictionaryWrapper<TKey, TValue, TInner> :
    Dictionary<TKey, TValue>,
    IDictionaryWrapper
    where TKey : notnull
    where TInner : IDictionary<TKey, TValue>
{
    public DictionaryWrapper(TInner inner) :
        base(inner.ToDictionary(_ => _.Key, _ => _.Value))
    {
    }
}