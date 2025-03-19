namespace VerifyTests;

public delegate Task BeforeCombination(IReadOnlyList<object?> keys);
public delegate Task AfterCombination(IReadOnlyList<object?> keys, object? result);
public delegate Task CombinationException(IReadOnlyList<object?> keys, Exception exception);

public static class CombinationSettings
{
    public static bool IncludeHeadersEnabled { get; private set; }

    public static void IncludeHeaders() =>
        IncludeHeadersEnabled = true;

    public static bool CaptureExceptionsEnabled { get; private set; }

    public static void CaptureExceptions() =>
        CaptureExceptionsEnabled = true;

    static BeforeCombination? before;

    internal static Task RunBeforeCallbacks(IReadOnlyList<object?> keys)
    {
        if (before == null)
        {
            return Task.CompletedTask;
        }

        return before(keys);
    }

    static AfterCombination? after;

    internal static Task RunAfterCallbacks(IReadOnlyList<object?> keys, object? result)
    {
        if (after == null)
        {
            return Task.CompletedTask;
        }

        return after(keys, result);
    }

    static CombinationException? combinationException;

    internal static Task RunExceptionCallbacks(IReadOnlyList<object?> keys, Exception exception)
    {
        if (combinationException == null)
        {
            return Task.CompletedTask;
        }

        return combinationException(keys, exception);
    }

    public static void UseCallbacks(BeforeCombination before, AfterCombination after, CombinationException exception)
    {
        CombinationSettings.before += before;
        CombinationSettings.after += after;
        combinationException += exception;
    }

    public static void Reset()
    {
        combinationException = null;
        after = null;
        before = null;
    }
}