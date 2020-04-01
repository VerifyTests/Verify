using System;
using Newtonsoft.Json;

namespace Verify
{
    public static partial class SharedVerifySettings
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

        public static void AddExtraDatetimeFormat(string format)
        {
            SharedScrubber.datetimeFormats.Add(format);
        }

        public static void AddExtraDatetimeOffsetFormat(string format)
        {
            SharedScrubber.datetimeOffsetFormats.Add(format);
        }
    }
}