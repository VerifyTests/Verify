class OrderedStringDictionaryWrapper<TValue, TInner> :
    Dictionary<string, TValue>,
    IDictionaryWrapper
    where TInner : IDictionary<string, TValue>
{
    public OrderedStringDictionaryWrapper(Func<string, bool> shouldIgnore, TInner inner) :
        base(inner
            .Where(_ => !shouldIgnore(_.Key))
            .OrderBy(_ => _.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(_ => _.Key, _ => _.Value))
    {
    }
}