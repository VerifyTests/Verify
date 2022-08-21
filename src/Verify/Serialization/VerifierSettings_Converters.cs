namespace VerifyTests;

public static partial class VerifierSettings
{
    public static void AddConverter<T>()
        where T : JsonConverter, new()
    {
        if (SerializationSettings.Converters.Any(_ => _.GetType() == typeof(T)))
        {
            return;
        }

        SerializationSettings.Converters.Add(new T());
    }

    public static void AddConverter(JsonConverter converter) =>
        SerializationSettings.Converters.Add(converter);

}