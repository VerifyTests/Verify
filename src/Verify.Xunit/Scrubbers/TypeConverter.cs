using System;
using Newtonsoft.Json;

namespace ObjectApproval
{
    public class TypeConverter :
        JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            var type = (Type) value;
            writer.WriteValue(TypeNameConverter.GetName(type));
        }

        public override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type type)
        {
            return typeof(Type).IsAssignableFrom(type);
        }
    }
}