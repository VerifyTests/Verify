namespace VerifyTests;

public static partial class Recording
{
    static HashSet<string> ignored = [];
    static readonly object ignoredLock = new();

    public static void IgnoreNames(params string[] names)
    {
        // Copy-on-write: IsIgnored reads this on every Recording.Add, potentially
        // from other threads, so publish a new set rather than mutating in place.
        lock (ignoredLock)
        {
            var copy = new HashSet<string>(ignored);
            foreach (var name in names)
            {
                copy.Add(name);
            }

            ignored = copy;
        }
    }

    public static bool IsIgnored(string name) =>
        Volatile.Read(ref ignored)
            .Contains(name);

    static AsyncLocal<State?> asyncLocal = new();

    public static void Add(string name, object item) =>
        CurrentState()
            .Add(name, item);

    public static bool NameExists(string name) =>
        CurrentState()
            .Items.Any(_ => _.Name == name);

    public static void TryAdd(string name, object item)
    {
        var value = asyncLocal.Value;
        value?.Add(name, item);
    }

    public static bool IsRecording()
    {
        var state = asyncLocal.Value;
        if (state == null)
        {
            return false;
        }

        return !state.Paused;
    }

    public static IReadOnlyCollection<ToAppend> Stop()
    {
        if (TryStop(out var values))
        {
            return values;
        }

        throw new("Recording.Start must be called prior to Recording.Stop.");
    }

    public static bool TryStop([NotNullWhen(true)] out IReadOnlyCollection<ToAppend>? recorded)
    {
        var value = asyncLocal.Value;

        if (value == null)
        {
            recorded = null;
            return false;
        }

        // Nulling asyncLocal only affects the current execution context. When the
        // verify engine consumes a recording mid-verify, that null does not flow
        // back to the calling test, which would otherwise keep seeing an active,
        // unconsumed recording (re-appending the same items on the next verify).
        // Snapshot the items, then stop the shared State so the stop is observable
        // through the caller's reference.
        recorded = value.Items.ToList();
        value.Clear();
        value.Pause();
        asyncLocal.Value = null;
        return true;
    }

    public static IReadOnlyCollection<ToAppend> Values()
    {
        if (TryGetValues(out var values))
        {
            return values;
        }

        throw new("Recording.Start must be called prior to Recording.Values.");
    }

    public static bool TryGetValues([NotNullWhen(true)] out IReadOnlyCollection<ToAppend>? recorded)
    {
        var value = asyncLocal.Value;

        if (value == null)
        {
            recorded = null;
            return false;
        }

        recorded = value.Items;
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

    public static IDisposable Start()
    {
        var value = asyncLocal.Value;

        if (value != null)
        {
            throw new("Recording already started");
        }

        asyncLocal.Value = new();
        return new Disposable();
    }

    class Disposable :
        IDisposable
    {
        // TryPause (not Pause) so disposing after an explicit Stop is a no-op
        // rather than throwing from CurrentState.
        public void Dispose() =>
            TryPause();
    }

    public static void Pause() =>
        CurrentState()
            .Pause();

    public static bool IsPaused()
    {
        var value = asyncLocal.Value;

        return value is {Paused: true};
    }

    public static void TryPause() =>
        asyncLocal.Value?.Pause();

    public static void Resume() =>
        CurrentState()
            .Resume();

    public static void TryResume() =>
        asyncLocal.Value?.Resume();

    public static void Clear() =>
        CurrentState()
            .Clear();

    public static void TryClear() =>
        asyncLocal.Value?.Clear();

    public static IReadOnlyDictionary<string, IReadOnlyList<object>> ToDictionary(this IEnumerable<ToAppend> values)
    {
        var dictionary = new Dictionary<string, IReadOnlyList<object>>(StringComparer.OrdinalIgnoreCase);

        foreach (var value in values)
        {
            if (!dictionary.TryGetValue(value.Name, out var item))
            {
                dictionary[value.Name] = item = new List<object>();
            }

            ((List<object>) item).Add(value.Data);
        }

        return dictionary;
    }
}
