using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VerifyTests
{
    public abstract class WriteOnlyJsonConverter<T> :
        WriteOnlyJsonConverter
    {
        public sealed override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer,
            IReadOnlyDictionary<string, object> context)
        {
            WriteJson(writer, (T) value, serializer, context);
        }

        public abstract void WriteJson(
            JsonWriter writer,
            T value,
            JsonSerializer serializer,
            IReadOnlyDictionary<string, object> context);

        static Type? nullableType;

        static WriteOnlyJsonConverter()
        {
            if (typeof(T).IsValueType)
            {
                nullableType = typeof(Nullable<>).MakeGenericType(typeof(T));
            }
        }

        public sealed override bool CanConvert(Type type)
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                return true;
            }

            return nullableType != null &&
                   nullableType == type;
        }
    }
}