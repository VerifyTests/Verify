class OrderedStringDictionaryWrapper<TValue, TInner> :
    Dictionary<string, TValue>,
    IDictionaryWrapper
    where TInner : IDictionary<string, TValue>
{
    public OrderedStringDictionaryWrapper(List<string> ignored, TInner inner) :
        base(inner
            .Where(_ => !ignored.Contains(_.Key))
            .OrderBy(_ => _.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(_ => _.Key, _ => _.Value))
    {
    }
}