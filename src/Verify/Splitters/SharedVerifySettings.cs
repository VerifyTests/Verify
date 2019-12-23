using System;
using System.Collections.Generic;
using System.IO;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        static Dictionary<string, StreamConverter> extensionConverters = new Dictionary<string, StreamConverter>();
        static Dictionary<Type, TypeConverter> typedConverters = new Dictionary<Type, TypeConverter>();

        internal static bool TryGetConverter(string extension, out StreamConverter converter)
        {
            return extensionConverters.TryGetValue(extension, out converter);
        }

        internal static bool TryGetConverter<T>(out TypeConverter converter)
        {
            return typedConverters.TryGetValue(typeof(T), out converter);
        }

        public static void RegisterFileConverter(
            string fromExtension,
            string toExtension,
            Func<Stream, IEnumerable<Stream>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstBadExtension(fromExtension, nameof(fromExtension));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            extensionConverters[fromExtension] = new StreamConverter(toExtension, func);
        }

        public static void RegisterFileConverter<T>(
            string toExtension,
            Func<T, IEnumerable<Stream>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            typedConverters[typeof(T)] = new TypeConverter(toExtension, o => func((T) o));
        }

        internal class TypeConverter
        {
            public string ToExtension { get; }
            public Func<object, IEnumerable<Stream>> Func { get; }

            public TypeConverter(string toExtension, Func<object, IEnumerable<Stream>> func)
            {
                ToExtension = toExtension;
                Func = func;
            }
        }

        internal class StreamConverter
        {
            public string ToExtension { get; }
            public Func<Stream, IEnumerable<Stream>> Func { get; }

            public StreamConverter(string toExtension, Func<Stream, IEnumerable<Stream>> func)
            {
                ToExtension = toExtension;
                Func = func;
            }
        }
    }
}