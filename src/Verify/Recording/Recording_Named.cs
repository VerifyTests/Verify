namespace VerifyTests;

public static partial class Recording
{
    static ConcurrentDictionary<string, State> namedState = new(StringComparer.OrdinalIgnoreCase);

    public static void Add(string identifier, string name, object item) =>
        CurrentStateNamed(identifier).Add(name, item);

    public static bool IsRecording(string identifier) =>
        namedState.ContainsKey(identifier);

    public static IReadOnlyDictionary<string, IReadOnlyList<object>> Stop(string identifier)
    {
        if (!TryStop(identifier, out var values))
        {
            throw new("Recording.Start must be called prior to Recording.Stop.");
        }

        return ToDictionary(values);
    }

    static IReadOnlyDictionary<string, IReadOnlyList<object>> ToDictionary(IReadOnlyCollection<ToAppend> values)
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

    public static void Start(string identifier)
    {
        if (!namedState.TryAdd(identifier, new()))
        {
            throw new("Recording already started");
        }
    }

    public static void Pause(string identifier) =>
        CurrentStateNamed(identifier).Pause();

    public static void Resume(string identifier) =>
        CurrentStateNamed(identifier).Resume();

    public static void Clear(string identifier) =>
        CurrentStateNamed(identifier).Clear();
}