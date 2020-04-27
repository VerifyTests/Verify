namespace Verify
{
    public readonly struct CompareResult
    {
        public bool IsEqual { get; }
        public string? Message { get; }

        public CompareResult(in bool isEqual, in string? message = null)
        {
            IsEqual = isEqual;
            Message = message;
        }
    }
}