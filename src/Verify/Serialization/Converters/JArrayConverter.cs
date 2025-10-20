class JArrayConverter :
    WriteOnlyJsonConverter<JArray>
{
    public override void Write(VerifyJsonWriter writer, JArray value)
    {
        var list = new List<object>();
        foreach (var item in value)
        {
            list.Add(item);
        }

        writer.Serialize(list);
    }
}