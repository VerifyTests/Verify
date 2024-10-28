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

    public VerifySettings UseStrictJson()
    {
        strictJson = true;
        return this;
    }

    public bool StrictJson => VerifierSettings.StrictJson ||
                              strictJson;

    bool strictJson;

    internal List<ToAppend> Appends = [];

    /// <summary>
    /// Append a key-value pair to the serialized target.
    /// </summary>
    public VerifySettings AppendValue(string name, object data)
    {
        Appends.Add(new(name, data));
        return this;
    }

    /// <summary>
    /// Append key-value pairs to the serialized target.
    /// </summary>
    public VerifySettings AppendValues(IEnumerable<KeyValuePair<string, object>> values)
    {
        foreach (var pair in values)
        {
            AppendValue(pair.Key, pair.Value);
        }

        return this;
    }

    /// <summary>
    /// Append key-value pairs to the serialized target.
    /// </summary>
    public VerifySettings AppendValues(params KeyValuePair<string, object>[] values)
    {
        foreach (var pair in values)
        {
            AppendValue(pair.Key, pair.Value);
        }

        return this;
    }

    public VerifySettings AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        CloneSettings();

        serialization.AddExtraSettings(action);

        return this;
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

    public VerifySettings DisableDateCounting()
    {
        dateCountingEnable = false;
        return this;
    }
}