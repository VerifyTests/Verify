class StringDictionaryWrapper<TValue, TInner> :
    Dictionary<string, TValue>,
    IDictionaryWrapper
    where TInner : IDictionary<string, TValue>
{
    public StringDictionaryWrapper(Func<string, bool> shouldIgnore, TInner inner) :
        base(inner
            .Where(_ => !shouldIgnore(_.Key))
            .ToDictionary(_ => _.Key, _ => _.Value))
    {
    }
}