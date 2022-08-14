partial class SerializationSettings
{
    bool ignoreEmptyCollections = true;

    public void DontIgnoreEmptyCollections() =>
        ignoreEmptyCollections = false;

    bool IsIgnoredCollection(Type memberType) =>
        ignoreEmptyCollections &&
        memberType.IsCollectionOrDictionary();
}