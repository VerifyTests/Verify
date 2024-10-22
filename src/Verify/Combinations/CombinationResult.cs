namespace VerifyTests;

public class CombinationResult
{
    public CombinationResult(object?[] keys, Exception exception)
    {
        Keys = keys;
        Exception = exception;
    }

    public CombinationResult(object?[] keys, object? value)
    {
        Keys = keys;
        Value = value;
    }

    public object?[] Keys { get; }
    public object? Value { get; }
    public Exception? Exception { get; }
}