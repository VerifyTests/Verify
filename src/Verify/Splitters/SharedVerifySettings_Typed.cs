using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        static List<TypeConverter> typedConverters = new List<TypeConverter>();

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

        public static void RegisterFileConverter<T>(
            string toExtension,
            Func<T, IEnumerable<Stream>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter<T>(toExtension, (target, settings) => func(target));
        }

        public static void RegisterFileConverter<T>(
            string toExtension,
            Func<T, VerifySettings, IEnumerable<Stream>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            var converter = new TypeConverter(
                toExtension,
                (o, settings) => func((T)o, settings),
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
    }
}