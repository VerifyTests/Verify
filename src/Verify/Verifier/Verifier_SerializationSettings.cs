using System;
using Newtonsoft.Json;
using Verify;

partial class Verifier
{
    SerializationSettings serialization = Global.serialization;
    bool isCloned;

    public void ModifySerialization(Action<SerializationSettings> action)
    {
        if (!isCloned)
        {
            serialization = Global.serialization.Clone();
        }

        action(serialization);
        serialization.RegenSettings();
    }

    public void AddExtraSettings(Action<JsonSerializerSettings> action)
    {
        if (!isCloned)
        {
            serialization = Global.serialization.Clone();
        }

        serialization.AddExtraSettings(action);
    }
}