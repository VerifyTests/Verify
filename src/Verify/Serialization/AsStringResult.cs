namespace VerifyTests;

public readonly struct AsStringResult(string value, string? extension = null)
{
    public string Value { get; } = value;
    public string? Extension { get; } = extension;

    public static implicit operator AsStringResult(string value) =>
        new(value);
}