namespace VerifyTests;

public readonly struct ToAppend
{
    public string Name { get; }
    public object Data { get; }

    public ToAppend(string name, object data)
    {
        Guard.AgainstBadExtension(name);
        Name = name;
        Data = data;
    }
}

class ToAppendConverter :
    WriteOnlyJsonConverter<ToAppend>
{
    public override void Write(VerifyJsonWriter writer, ToAppend value)
    {
        writer.WritePropertyName(value.Name);
        writer.WriteValue(value.Data);
    }
}