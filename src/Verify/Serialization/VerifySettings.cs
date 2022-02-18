namespace VerifyTests;

public partial class VerifySettings
{
    internal SerializationSettings serialization = VerifierSettings.serialization;
    bool isCloned;

    public void ModifySerialization(Action<SerializationSettings> action)
    {
        CloneSettings();

        action(serialization);
    }

    void CloneSettings()
    {
        if (isCloned)
        {
            return;
        }
        serialization = new(serialization);
        isCloned = true;
    }

    internal JsonSerializer Serializer
    {
        get { return serialization.Serializer; }
    }

    internal List<ToAppend> Appends = new();

    /// <summary>
    /// Append a key-value pair to the serialized target.
    /// </summary>
    public void AppendValue(string name, object data)
    {
        Appends.Add(new(name, data));
    }

    /// <summary>
    /// Append key-value pairs to the serialized target.
    /// </summary>
    public void AppendValues(IEnumerable<KeyValuePair<string, object>> values)
    {
        foreach (var pair in values)
        {
            AppendValue(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// Append key-value pairs to the serialized target.
    /// </summary>
    public void AppendValues(params KeyValuePair<string, object>[] values)
    {
        foreach (var pair in values)
        {
            AppendValue(pair.Key, pair.Value);
        }
    }

    public void IgnoreStackTrack()
    {
        ModifySerialization(_ => _.IgnoreMember("StackTrace"));
    }

    public void AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        CloneSettings();

        serialization.AddExtraSettings(action);
    }
}