﻿namespace VerifyTests;

public class CombinationResult
{
    public static CombinationResult ForException(IReadOnlyList<object?> keys, Exception exception) =>
        new(keys, exception);

    CombinationResult(IReadOnlyList<object?> keys, Exception exception)
    {
        Keys = keys;
        Type = CombinationResultType.Exception;
        this.exception = exception;
    }

    public static CombinationResult ForValue(IReadOnlyList<object?> keys, object? value) =>
        new(keys, value);

    CombinationResult(IReadOnlyList<object?> keys, object? value)
    {
        Keys = keys;
        Type = CombinationResultType.Value;
        this.value = value;
    }

    public static CombinationResult ForVoid(IReadOnlyList<object?> keys) =>
        new(keys);

    CombinationResult(IReadOnlyList<object?> keys)
    {
        Keys = keys;
        Type = CombinationResultType.Void;
    }

    public CombinationResultType Type { get; }
    public IReadOnlyList<object?> Keys { get; }

    readonly object? value;

    public object? Value
    {
        get
        {
            if (Type != CombinationResultType.Value)
            {
                throw new($"Invalid CombinationResultType: {Type}");
            }

            return value;
        }
    }

    Exception? exception;

    public Exception Exception
    {
        get
        {
            if (Type != CombinationResultType.Exception)
            {
                throw new($"Invalid CombinationResultType: {Type}");
            }

            return exception!;
        }
    }
}