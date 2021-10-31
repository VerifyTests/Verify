using Newtonsoft.Json.Serialization;
using SimpleInfoName;

class ShortNameBinder :
    ISerializationBinder
{
    public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
    {
        assemblyName = null;
        typeName = serializedType.SimpleName();
    }

    public Type BindToType(string? assemblyName, string typeName)
    {
        throw new();
    }
}