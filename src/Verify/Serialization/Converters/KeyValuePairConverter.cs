class KeyValuePairConverter :
    WriteOnlyJsonConverter
{
    public override bool CanConvert(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var definition = type.GetGenericTypeDefinition();
        return definition == typeof(KeyValuePair<,>) &&
               type.GetGenericArguments().First() == typeof(string);
    }

    public override void Write(VerifyJsonWriter writer, object value)
    {
        writer.WriteStartObject();

        var keyMember = value.GetType().GetProperty("Key")!.GetMethod!.Invoke(value, null)!;
        var valueMember = value.GetType().GetProperty("Value")!.GetMethod!.Invoke(value, null);
        writer.WriteMember(value, valueMember, (string) keyMember);

        writer.WriteEndObject();
    }
}