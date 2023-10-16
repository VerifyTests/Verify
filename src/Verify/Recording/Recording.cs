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

        var readOnlyCollection = value.Items;
        asyncLocal.Value = null;
        return readOnlyCollection;
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
}