namespace VerifyTests;

public abstract class WriteOnlyJsonConverter<T> :
    WriteOnlyJsonConverter
{
    public sealed override void Write(VerifyJsonWriter writer, object value)
    {
        Write(writer, (T) value);
    }

    public abstract void Write(VerifyJsonWriter writer, T value);

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