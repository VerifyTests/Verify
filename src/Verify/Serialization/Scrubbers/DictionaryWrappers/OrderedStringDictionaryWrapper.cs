class OrderedStringDictionaryWrapper<TValue, TInner> :
    Dictionary<string, TValue>,
    IDictionaryWrapper
    where TInner : IDictionary<string, TValue>
{
    public OrderedStringDictionaryWrapper(List<string> ignored, TInner inner) :
        base(inner.Where(x => !ignored.Contains(x.Key))
            .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.Value))
    {
    }
}