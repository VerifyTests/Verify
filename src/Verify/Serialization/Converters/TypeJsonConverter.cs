using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class TypeJsonConverter :
    WriteOnlyJsonConverter<Type>
{
    public override void WriteJson(
        JsonWriter writer,
        Type value,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteValue(value.SimpleName());
    }
}