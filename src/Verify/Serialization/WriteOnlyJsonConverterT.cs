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

        static Type nullableType = typeof(Nullable<>);

        public sealed override bool CanConvert(Type type)
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                return true;
            }

            if (typeof(T).IsValueType)
            {
                var genericType = nullableType.MakeGenericType(typeof(T));
                return genericType.IsAssignableFrom(type);
            }
            return false;
        }
    }
}