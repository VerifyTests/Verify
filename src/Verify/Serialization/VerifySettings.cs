namespace VerifyTests;

public partial class VerifySettings
{
    internal SerializationSettings serialization = VerifierSettings.serialization;
    bool isCloned;

    void CloneSettings()
    {
        if (isCloned)
        {
            return;
        }

        serialization = new(serialization);
        isCloned = true;
    }

    internal JsonSerializer Serializer => serialization.Serializer;


    internal List<ToAppend> Appends = [];

    /// <summary>
    /// Append a key-value pair to the serialized target.
    /// </summary>
    public void AppendValue(string name, object data) =>
        Appends.Add(new(name, data));

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

    public void AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        CloneSettings();

        serialization.AddExtraSettings(action);
    }

    bool dateCountingEnable = true;

    public bool DateCountingEnable
    {
        get
        {
            if (!VerifierSettings.DateCountingEnabled)
            {
                return false;
            }

            return dateCountingEnable;
        }
    }

    public void DisableDateCounting() =>
        dateCountingEnable = false;
}