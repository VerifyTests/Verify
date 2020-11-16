using System.Diagnostics;

namespace VerifyTests
{
    [DebuggerDisplay("IsEqual = {IsEqual} | Message = {Message}")]
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

        public static CompareResult Equal = new(true);

        public static CompareResult NotEqual(in string? message = null)
        {
            return new(false, message);
        }
    }
}