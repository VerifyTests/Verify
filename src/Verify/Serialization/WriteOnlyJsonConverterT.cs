namespace VerifyTests;

public abstract class WriteOnlyJsonConverter<T> :
    WriteOnlyJsonConverter
{
    public sealed override void Write(VerifyJsonWriter writer, object value) =>
        Write(writer, (T) value);

    public abstract void Write(VerifyJsonWriter writer, T value);

    Type? nullableType;

    protected WriteOnlyJsonConverter()
    {
        if (typeof(T).IsValueType)
        {
            nullableType = typeof(Nullable<>).MakeGenericType(typeof(T));
        }
    }

    public sealed override bool CanConvert(Type type)
    {
        if (type.IsAssignableTo<T>())
        {
            return true;
        }

        return nullableType is not null &&
               nullableType == type;
    }
}