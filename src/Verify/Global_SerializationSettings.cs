using System;
using Newtonsoft.Json;

namespace Verify
{
    public static partial class Global
    {
        internal static SerializationSettings serialization = new SerializationSettings();

        public static void AddExtraSettings(Action<JsonSerializerSettings> action)
        {
            serialization.AddExtraSettings(action);
            serialization.RegenSettings();
        }

        public static void ModifySerialization(Action<SerializationSettings> action)
        {
            action(serialization);
            serialization.RegenSettings();
        }
    }
}