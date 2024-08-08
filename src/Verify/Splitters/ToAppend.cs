namespace VerifyTests;

public readonly struct ToAppend
{
    public string Name { get; }
    public object Data { get; }

    public ToAppend(string name, object data)
    {
        Guards.AgainstBadExtension(name);
        Name = name;
        Data = data;
    }
}