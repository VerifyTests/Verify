namespace VerifyTests
{
    public readonly struct AsStringResult
    {
        public string Value { get; }
        public string? Extension { get; }

        public AsStringResult(string value, string? extension = null)
        {
            Value = value;
            Extension = extension;
        }

        public static implicit operator AsStringResult(string value)
        {
            return new(value);
        }
    }
}