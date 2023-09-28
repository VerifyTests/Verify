class NoOpNameTable :
    JsonNameTable
{
    public static readonly NoOpNameTable Instance = new();

    public override string? Get(char[] key, int start, int length) => null;

    public override string? Add(string name) => null;
}