using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ObjectApproval
{
    public class Scrubber<T> : JsonConverter where T : struct
    {
        static string name = typeof(T).Name;
        int count;
        Dictionary<T, int> cache = new Dictionary<T, int>();

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }
            WriteValue(writer, (T)value);
        }

        public void WriteValue(JsonWriter writer, T value)
        {
            if (cache.TryGetValue(value, out var cachedCount))
            {
                writer.WriteRawValue($"{name}_{cachedCount}");
                return;
            }
            count++;
            cache[value] = count;
            writer.WriteRawValue($"{name}_{count}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new Exception();
        }

        public override bool CanRead => false;

        public override bool CanConvert(Type type)
        {
            return type == typeof(T) ||
                   type == typeof(T?);
        }
    }
}