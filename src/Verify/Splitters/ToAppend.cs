namespace VerifyTests;

public readonly struct ToAppend
{
    public string Name { get; }
    public object Data { get; }

    public ToAppend(string name, object data)
    {
        Guard.NotNullOrWhiteSpace(name);
        Name = name;
        Data = data;
    }
}