namespace VerifyTests;

public static class Recording
{
    static AsyncLocal<State?> asyncLocal = new();

    public static void Add(string name, object item)
    {
        Guard.AgainstBadExtension(name);
        CurrentState().Add(name, item);
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

    static State CurrentState([CallerMemberName] string caller = "")
    {
        var value = asyncLocal.Value;

        if (value != null)
        {
            return value;
        }

        throw new($"Recording.Start must be called before Recording.{caller}");
    }

    public static void Start()
    {
        var value = asyncLocal.Value;

        if (value != null)
        {
            throw new("Recording already started");
        }

        asyncLocal.Value = new();
    }

    public static void Pause() =>
        CurrentState().Pause();

    public static void Resume() =>
        CurrentState().Resume();

    public static void Clear() =>
        CurrentState().Clear();
}