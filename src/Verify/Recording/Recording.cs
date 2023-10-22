namespace VerifyTests;

public static class Recording
{
    static AsyncLocal<State?> asyncLocal = new();

    public static void Add(string name, object item)
    {
        Guard.AgainstBadExtension(name);
        var state = CurrentState();
        state.Add(name, item);
    }

    public static IReadOnlyCollection<ToAppend> Stop()
    {
        var value = asyncLocal.Value;

        if (value == null)
        {
            return Array.Empty<ToAppend>();
        }

        var items = value.Items;
        asyncLocal.Value = null;
        return items;
    }

    static State CurrentState()
    {
        var value = asyncLocal.Value;

        if (value == null)
        {
            return asyncLocal.Value = new();
        }

        return value;
    }

    public static void Pause()
    {
        var state = CurrentState();
        state.Pause();
    }
    public static void Resume()
    {
        var state = CurrentState();
        state.Resume();
    }

    public static void Clear()
    {
        var state = CurrentState();
        state.Clear();
    }
}