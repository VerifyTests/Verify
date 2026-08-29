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

    // Held as lists rather than combined with `+=`. Invoking a multicast Task returning
    // delegate returns only the last target's Task, so every earlier callback would run
    // unawaited: its exceptions unobserved, and its async work racing the combination
    // method. Same hazard the Then extension documents for Func<Task>.
    static List<BeforeCombination>? before;

    internal static Task RunBeforeCallbacks(IReadOnlyList<object?> keys)
    {
        if (before == null)
        {
            return Task.CompletedTask;
        }

        return RunAll(before, keys);
    }

    static async Task RunAll(List<BeforeCombination> callbacks, IReadOnlyList<object?> keys)
    {
        foreach (var callback in callbacks)
        {
            await callback(keys);
        }
    }

    static List<AfterCombination>? after;

    internal static Task RunAfterCallbacks(IReadOnlyList<object?> keys, object? result)
    {
        if (after == null)
        {
            return Task.CompletedTask;
        }

        return RunAll(after, keys, result);
    }

    static async Task RunAll(List<AfterCombination> callbacks, IReadOnlyList<object?> keys, object? result)
    {
        foreach (var callback in callbacks)
        {
            await callback(keys, result);
        }
    }

    static List<CombinationException>? combinationException;

    internal static Task RunExceptionCallbacks(IReadOnlyList<object?> keys, Exception exception)
    {
        if (combinationException == null)
        {
            return Task.CompletedTask;
        }

        return RunAll(combinationException, keys, exception);
    }

    static async Task RunAll(List<CombinationException> callbacks, IReadOnlyList<object?> keys, Exception exception)
    {
        foreach (var callback in callbacks)
        {
            await callback(keys, exception);
        }
    }

    public static void UseCallbacks(BeforeCombination before, AfterCombination after, CombinationException exception)
    {
        CombinationSettings.before ??= [];
        CombinationSettings.before.Add(before);
        CombinationSettings.after ??= [];
        CombinationSettings.after.Add(after);
        combinationException ??= [];
        combinationException.Add(exception);
    }

    public static void Reset()
    {
        combinationException = null;
        IncludeHeadersEnabled = false;
        CaptureExceptionsEnabled = false;
        after = null;
        before = null;
    }
}
