namespace VerifyTests;

public static partial class Recording
{
    static ConcurrentDictionary<string, State> namedState = new(StringComparer.OrdinalIgnoreCase);

    public static void Add(string identifier, string name, object item) =>
        CurrentStateNamed(identifier)
            .Add(name, item);

    public static void TryAdd(string identifier, string name, object item)
    {
        if (namedState.TryGetValue(identifier, out var state))
        {
            state.Add(name, item);
        }
    }

    public static bool IsRecording(string identifier)
    {
        if (!namedState.TryGetValue(identifier, out var state))
        {
            return false;
        }

        return !state.Paused;
    }

    public static IReadOnlyCollection<ToAppend> Stop(string identifier)
    {
        if (TryStop(identifier, out var values))
        {
            return values;
        }

        throw new("Recording.Start(string identifier) must be called prior to Recording.Stop(string identifier).");
    }

    public static bool TryStop(
        string identifier,
        [NotNullWhen(true)] out IReadOnlyCollection<ToAppend>? recorded)
    {
        if (namedState.TryRemove(identifier, out var state))
        {
            recorded = state.Items;
            return true;
        }

        recorded = null;
        return false;
    }

    static State CurrentStateNamed(string identifier, [CallerMemberName] string caller = "")
    {
        if (namedState.TryGetValue(identifier, out var state))
        {
            return state;
        }

        throw new($"Recording.Start(string identifier) must be called before Recording.{caller}");
    }

    public static IDisposable Start(string identifier)
    {
        if (!namedState.TryAdd(identifier, new()))
        {
            throw new("Recording already started");
        }

        return new NamedDisposable(identifier);
    }

    class NamedDisposable(string identifier) :
        IDisposable
    {
        public void Dispose() =>
            Pause(identifier);
    }

    public static void Pause(string identifier) =>
        CurrentStateNamed(identifier)
            .Pause();

    public static void TryPause(string identifier)
    {
        if (namedState.TryGetValue(identifier, out var state))
        {
            state.Pause();
        }
    }

    public static void Resume(string identifier) =>
        CurrentStateNamed(identifier)
            .Resume();

    public static void TryResume(string identifier)
    {
        if (namedState.TryGetValue(identifier, out var state))
        {
            state.Resume();
        }
    }

    public static void Clear(string identifier) =>
        CurrentStateNamed(identifier)
            .Clear();

    public static void TryClear(string identifier)
    {
        if (namedState.TryGetValue(identifier, out var state))
        {
            state.Clear();
        }
    }
}