using Newtonsoft.Json;

namespace VerifyTests;

public abstract class WriteOnlyJsonConverter<T> :
    WriteOnlyJsonConverter
{
    public sealed override void Write(
        VerifyJsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        Write(writer, (T) value, serializer);
    }

    public abstract void Write(
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