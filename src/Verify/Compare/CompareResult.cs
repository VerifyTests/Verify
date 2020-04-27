namespace Verify
{
    public readonly struct CompareResult
    {
        public bool IsEqual { get; }
        public string? Message { get; }

        CompareResult(in bool isEqual, in string? message = null)
        {
            IsEqual = isEqual;
            Message = message;
        }

        public CompareResult(in bool isEqual)
        {
            IsEqual = isEqual;
            Message = null;
        }

        public static CompareResult Equal = new CompareResult(true);

        public static CompareResult NotEqual(in string? message = null)
        {
            return new CompareResult(false, message);
        }
    }
}