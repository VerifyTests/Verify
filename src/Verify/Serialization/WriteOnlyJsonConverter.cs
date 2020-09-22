using System;
using Newtonsoft.Json;

namespace VerifyTests
{
    public abstract class WriteOnlyJsonConverter :
        JsonConverter
    {
        public override bool CanRead => false;

        public sealed override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}