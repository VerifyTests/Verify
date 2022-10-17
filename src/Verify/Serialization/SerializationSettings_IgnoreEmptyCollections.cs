partial class SerializationSettings
{
    bool ignoreEmptyCollections = true;

    public void DontIgnoreEmptyCollections() =>
        ignoreEmptyCollections = false;
}