namespace VerifyTests;

public class CombinationResult
{
    public CombinationResult(IReadOnlyList<object?> keys, Exception exception)
    {
        Keys = keys;
        Exception = exception;
    }

    public CombinationResult(IReadOnlyList<object?> keys, object? value)
    {
        Keys = keys;
        Value = value;
    }

    public IReadOnlyList<object?> Keys { get; }
    public object? Value { get; }
    public Exception? Exception { get; }
}