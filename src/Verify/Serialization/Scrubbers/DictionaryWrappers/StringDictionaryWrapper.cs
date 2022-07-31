class StringDictionaryWrapper<TValue, TInner> :
    Dictionary<string, TValue>,
    IDictionaryWrapper
    where TInner : IDictionary<string, TValue>
{
    public StringDictionaryWrapper(List<string> ignored, TInner inner) :
        base(inner
            .Where(_ => !ignored.Contains(_.Key))
            .ToDictionary(_ => _.Key, _ => _.Value))
    {
    }
}