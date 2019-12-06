using System;
using Newtonsoft.Json;

namespace VerifyXunit
{
    public abstract class WriteOnlyJsonConverter :
        JsonConverter
    {
        public sealed override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}