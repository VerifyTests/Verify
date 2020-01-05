using System;
using Newtonsoft.Json;

namespace Verify
{
    public abstract class WriteOnlyJsonConverter :
        JsonConverter
    {
        public sealed override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class WriteOnlyJsonConverter<T> :
        WriteOnlyJsonConverter
        where T: class
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            WriteJson(writer, (T?) value, serializer);
        }

        public abstract void WriteJson(JsonWriter writer, T? value, JsonSerializer serializer);

        public override bool CanConvert(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }
    }
}