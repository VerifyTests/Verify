class KeyValuePairConverter :
    WriteOnlyJsonConverter
{
    public override bool CanConvert(Type type) =>
        type.IsGeneric(typeof(KeyValuePair<,>)) &&
        type
            .GetGenericArguments()[0] == typeof(string);

    public override void Write(VerifyJsonWriter writer, object value)
    {
        writer.WriteStartObject();

        var type = value.GetType();
        var keyMember = type.GetProperty("Key")!.GetMethod!.Invoke(value, null)!;
        var valueMember = type.GetProperty("Value")!.GetMethod!.Invoke(value, null);
        writer.WriteMember(value, valueMember, (string) keyMember);

        writer.WriteEndObject();
    }
}