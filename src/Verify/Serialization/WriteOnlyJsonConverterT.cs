using System;
using Newtonsoft.Json;

namespace VerifyTests
{
    public abstract class WriteOnlyJsonConverter<T> :
        WriteOnlyJsonConverter
        where T: class
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            WriteJson(writer, (T?) value, serializer);
        }

        public abstract void WriteJson(JsonWriter writer, T? fragment, JsonSerializer serializer);

        public override bool CanConvert(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}