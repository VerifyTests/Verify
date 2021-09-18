class DictionaryWrapper<TKey, TValue> :
    Dictionary<TKey, TValue>,
    IDictionaryWrapper
    where TKey : notnull
{

}