using Newtonsoft.Json;

namespace VerifyTests;

public partial class VerifySettings
{
    internal SerializationSettings serialization = VerifierSettings.serialization;
    bool isCloned;

    public void ModifySerialization(Action<SerializationSettings> action)
    {
        if (!isCloned)
        {
            serialization = serialization.Clone();
            isCloned = true;
        }

        action(serialization);
        serialization.RegenSettings();
    }

    internal List<ToAppend> Appends = new();

    public void AppendValue(string name, object data)
    {
        Appends.Add(new(name, data));
    }

    public void IgnoreStackTrack()
    {
        ModifySerialization(_ => _.IgnoreMember("StackTrace"));
    }

    public void AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        if (!isCloned)
        {
            serialization = serialization.Clone();
            isCloned = true;
        }

        serialization.AddExtraSettings(action);
        serialization.RegenSettings();
    }
}