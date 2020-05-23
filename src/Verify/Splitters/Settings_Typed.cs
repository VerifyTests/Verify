using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        static List<TypeConverter> typedConverters = new List<TypeConverter>();

        internal static bool TryGetConverter<T>(
            string? extension,
            [NotNullWhen(true)] out TypeConverter? converter)
        {
            if (extension != null)
            {
                foreach (var typedConverter in typedConverters
                    .Where(_ => _.ToExtension != null &&
                                _.ToExtension == extension &&
                                _.CanConvert(typeof(T))))
                {
                    converter = typedConverter;
                    return true;
                }
            }

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
            ObjectConversion<T> func,
            Func<Type, bool>? canConvert = null)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter<T>(
                (o, settings) => Task.FromResult(func(o, settings)),
                canConvert);
        }

        public static void RegisterFileConverter<T>(
            AsyncObjectConversion<T> func,
            Func<Type, bool>? canConvert = null)
        {
            Guard.AgainstNull(func, nameof(func));
            canConvert = DefaultCanConvert<T>(canConvert);
            var converter = new TypeConverter(
                (o, settings) => func((T) o, settings),
                canConvert);
            typedConverters.Add(converter);
        }

        public static void RegisterFileConverter(
            ObjectConversion<object> func,
            Func<Type, bool> canConvert)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter(
                (o, settings) => Task.FromResult(func(o, settings)),
                canConvert);
        }

        public static void RegisterFileConverter(
            AsyncObjectConversion<object> func,
            Func<Type, bool> canConvert)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstNull(canConvert, nameof(canConvert));
            var converter = new TypeConverter(
                func,
                canConvert);
            typedConverters.Add(converter);
        }

        public static void RegisterFileConverter<T>(
            string toExtension,
            ObjectConversion<T> func,
            Func<Type, bool>? canConvert = null)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter<T>(
                toExtension,
                (o, settings) => Task.FromResult(func(o, settings)),
                canConvert);
        }

        public static void RegisterFileConverter<T>(
            string toExtension,
            AsyncObjectConversion<T> func,
            Func<Type, bool>? canConvert = null)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            canConvert = DefaultCanConvert<T>(canConvert);

            var converter = new TypeConverter(
                toExtension,
                (o, settings) => func((T) o, settings),
                canConvert);
            typedConverters.Add(converter);
        }

        static Func<Type, bool> DefaultCanConvert<T>(Func<Type, bool>? canConvert)
        {
            if (canConvert != null)
            {
                return canConvert;
            }
            return type => typeof(T).IsAssignableFrom(type);
        }

        public static void RegisterFileConverter(
            string toExtension,
            ObjectConversion<object> func,
            Func<Type, bool> canConvert)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter(
                toExtension,
                (o, settings) => Task.FromResult(func(o, settings)),
                canConvert);
        }

        public static void RegisterFileConverter(
            string toExtension,
            AsyncObjectConversion<object> func,
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