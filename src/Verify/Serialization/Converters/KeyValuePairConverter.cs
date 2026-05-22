class KeyValuePairConverter :
    WriteOnlyJsonConverter
{
    static ConcurrentDictionary<Type, Action<VerifyJsonWriter, object>> writers = new();

    public override bool CanConvert(Type type) =>
        type.IsGeneric(typeof(KeyValuePair<,>)) &&
        type
            .GetGenericArguments()[0] == typeof(string);

    public override void Write(VerifyJsonWriter writer, object value) =>
        writers.GetOrAdd(value.GetType(), BuildWriter)(writer, value);

    static Action<VerifyJsonWriter, object> BuildWriter(Type type)
    {
        var valueType = type.GetGenericArguments()[1];
        var method = typeof(KeyValuePairConverter)
            .GetMethod(nameof(WriteTyped), BindingFlags.Static | BindingFlags.NonPublic)!
            .MakeGenericMethod(valueType);
        return (Action<VerifyJsonWriter, object>) method.CreateDelegate(typeof(Action<VerifyJsonWriter, object>));
    }

    static void WriteTyped<TValue>(VerifyJsonWriter writer, object value)
    {
        var pair = (KeyValuePair<string, TValue>) value;
        writer.WriteStartObject();
        writer.WriteMember(value, (object?) pair.Value, pair.Key);
        writer.WriteEndObject();
    }
}
