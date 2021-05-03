using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VerifyTests
{
    public abstract class WriteOnlyJsonConverter :
        JsonConverter
    {
        public sealed override bool CanRead => false;

        public sealed override void WriteJson(
            JsonWriter writer,
            object? value,
            JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            var writerEx = (JsonTextWriterEx)writer;

            WriteJson(writer, value, serializer, writerEx.Context);
        }

        public abstract void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer,
            IReadOnlyDictionary<string, object> context);

        public sealed override object ReadJson(
            JsonReader reader,
            Type type,
            object? value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}