namespace VerifyTests;

public static partial class Recording
{
    static List<string> ignored = [];

    public static void IgnoreNames(params string[] names) =>
        ignored.AddRange(names);

    public static bool IsIgnored(string name) =>
        ignored.Contains(name);

    static AsyncLocal<State?> asyncLocal = new();

    public static void Add(string name, object item) =>
        CurrentState()
            .Add(name, item);

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
        public void Dispose() =>
            Pause();
    }

    public static void Pause() =>
        CurrentState()
            .Pause();

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
            List<object> objects;
            if (dictionary.TryGetValue(value.Name, out var item))
            {
                objects = (List<object>) item;
            }
            else
            {
                dictionary[value.Name] = objects = new();
            }

            objects.Add(value.Data);
        }

        return dictionary;
    }
}