using Newtonsoft.Json;

namespace VerifyTests;

public abstract class WriteOnlyJsonConverter<T> :
    WriteOnlyJsonConverter
{
    public sealed override void WriteJson(
        VerifyJsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        WriteJson(writer, (T) value, serializer);
    }

    public abstract void WriteJson(
        VerifyJsonWriter writer,
        T value,
        JsonSerializer serializer);

    static Type? nullableType;

    static WriteOnlyJsonConverter()
    {
        if (typeof(T).IsValueType)
        {
            nullableType = typeof(Nullable<>).MakeGenericType(typeof(T));
        }
    }

    public sealed override bool CanConvert(Type type)
    {
        if (typeof(T).IsAssignableFrom(type))
        {
            return true;
        }

        return nullableType != null &&
               nullableType == type;
    }
}