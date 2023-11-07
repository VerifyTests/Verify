namespace VerifyTests;

public static partial class Recording
{
    static AsyncLocal<State?> asyncLocal = new();

    public static void Add(string name, object item) =>
        CurrentState().Add(name, item);

    public static bool IsRecording() =>
        asyncLocal.Value != null;

    public static IReadOnlyDictionary<string, IReadOnlyList<object>> Stop()
    {
        if (TryStop(out var values))
        {
            return ToDictionary(values);
        }

        throw new("Recording.Start must be called prior to Recording.Stop.");
    }

    internal static bool TryStop([NotNullWhen(true)] out IReadOnlyCollection<ToAppend>? recorded)
    {
        var value = asyncLocal.Value;

        if (value == null)
        {
            recorded = null;
            return false;
        }

        recorded = value.Items;
        asyncLocal.Value = null;
        return true;
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