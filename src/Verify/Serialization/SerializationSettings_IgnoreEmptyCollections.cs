partial class SerializationSettings
{
    bool ignoreEmptyCollections = true;

    //TODO: move ignoreEmptyCollections to Argon
    public void DontIgnoreEmptyCollections() =>
        ignoreEmptyCollections = false;
}