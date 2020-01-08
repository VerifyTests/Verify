using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        static Dictionary<string, StreamConverter> extensionConverters = new Dictionary<string, StreamConverter>();
        static List<TypeConverter> typedConverters = new List<TypeConverter>();

        internal static bool TryGetConverter(string extension, out StreamConverter converter)
        {
            return extensionConverters.TryGetValue(extension, out converter);
        }

        internal static bool TryGetConverter<T>([NotNullWhen(true)] out TypeConverter? converter)
        {
            foreach (var typedConverter in typedConverters
                .Where(_ => _.CanConvert(typeof(T))))
            {
                converter = typedConverter;
                return true;
            }

            converter = null;
            return false;
        }

        public static void RegisterFileConverter(
            string fromExtension,
            string toExtension,
            Func<Stream, IEnumerable<Stream>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter(fromExtension, toExtension, (stream, settings) => func(stream));
        }

        public static void RegisterFileConverter(
            string fromExtension,
            string toExtension,
            Func<Stream, VerifySettings, IEnumerable<Stream>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstBadExtension(fromExtension, nameof(fromExtension));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            var converter = new StreamConverter(
                toExtension,
                func);
            extensionConverters[fromExtension] = converter;
        }

        public static void RegisterFileConverter<T>(
            string toExtension,
            Func<T, IEnumerable<Stream>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter<T>(toExtension, (target, settings) => func((T) target));
        }

        public static void RegisterFileConverter<T>(
            string toExtension,
            Func<object, VerifySettings, IEnumerable<Stream>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            var converter = new TypeConverter(
                toExtension,
                func,
                type => type == typeof(T));
            typedConverters.Add(converter);
        }

        public static void RegisterFileConverter(
            string toExtension,
            Func<object, IEnumerable<Stream>> func,
            Func<Type, bool> canConvert)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter(toExtension, (o,x) => func(o), canConvert);
        }

        public static void RegisterFileConverter(
            string toExtension,
            Func<object, VerifySettings, IEnumerable<Stream>> func,
            Func<Type, bool> canConvert)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstNull(canConvert, nameof(canConvert));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            var converter = new TypeConverter(
                toExtension,
                func,
                canConvert);
            typedConverters.Add(converter);
        }

        internal class TypeConverter
        {
            public string ToExtension { get; }
            public Func<object, VerifySettings, IEnumerable<Stream>> Func { get; }
            public Func<Type, bool> CanConvert { get; }

            public TypeConverter(string toExtension, Func<object, VerifySettings, IEnumerable<Stream>> func, Func<Type, bool> canConvert)
            {
                ToExtension = toExtension;
                Func = func;
                CanConvert = canConvert;
            }
        }

        internal class StreamConverter
        {
            public string ToExtension { get; }
            public Func<Stream, VerifySettings, IEnumerable<Stream>> Func { get; }

            public StreamConverter(string toExtension, Func<Stream, VerifySettings, IEnumerable<Stream>> func)
            {
                ToExtension = toExtension;
                Func = func;
            }
        }
    }
}