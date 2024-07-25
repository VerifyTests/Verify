namespace VerifyTests;

public static partial class Recording
{
    static AsyncLocal<ConcurrentDictionary<string, State>?> asyncLocalNamed = new();

    public static void Add(string identifier, string name, object item) =>
        CurrentStateNamed(identifier)
            .Add(name, item);

    public static void TryAdd(string identifier, string name, object item)
    {
        var states = asyncLocalNamed.Value;
        if (states != null)
        {
            if (states.TryGetValue(identifier, out var state))
            {
                state.Add(name, item);
            }
        }
    }

    public static bool IsRecording(string identifier)
    {
        var states = asyncLocalNamed.Value;
        if (states == null)
        {
            return false;
        }

        if (!states.TryGetValue(identifier, out var state))
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
        var states = asyncLocalNamed.Value;
        if (states != null)
        {
            if (states.TryRemove(identifier, out var state))
            {
                recorded = state.Items;
                return true;
            }
        }

        recorded = null;
        return false;
    }

    static State CurrentStateNamed(string identifier, [CallerMemberName] string caller = "")
    {
        var states = asyncLocalNamed.Value;
        if (states != null &&
            states.TryGetValue(identifier, out var state))
        {
            return state;
        }

        throw new($"Recording.Start(string identifier) must be called before Recording.{caller}");
    }

    public static IDisposable Start(string identifier)
    {
        var states = asyncLocalNamed.Value ??= new(StringComparer.OrdinalIgnoreCase);

        if (!states.TryAdd(identifier, new()))
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
        var states = asyncLocalNamed.Value;
        if (states == null)
        {
            return;
        }

        if (states.TryGetValue(identifier, out var state))
        {
            state.Pause();
        }
    }

    public static void Resume(string identifier) =>
        CurrentStateNamed(identifier)
            .Resume();

    public static void TryResume(string identifier)
    {
        var states = asyncLocalNamed.Value;
        if (states == null)
        {
            return;
        }

        if (states.TryGetValue(identifier, out var state))
        {
            state.Resume();
        }
    }

    public static void Clear(string identifier) =>
        CurrentStateNamed(identifier)
            .Clear();

    public static void TryClear(string identifier)
    {
        var states = asyncLocalNamed.Value;
        if (states == null)
        {
            return;
        }

        if (states.TryGetValue(identifier, out var state))
        {
            state.Clear();
        }
    }
}