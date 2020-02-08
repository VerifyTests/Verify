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
            Func<T, VerifySettings, ConversionResult> func)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter<T>((o, settings) => Task.FromResult(func(o, settings)));
        }

        public static void RegisterFileConverter<T>(
            Func<T, VerifySettings, Task<ConversionResult>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            var converter = new TypeConverter(
                (o, settings) => func((T) o, settings),
                type => typeof(T).IsAssignableFrom(type));
            typedConverters.Add(converter);
        }

        public static void RegisterFileConverter(
            Func<object, VerifySettings, ConversionResult> func,
            Func<Type, bool> canConvert)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter(
                (o, settings) => Task.FromResult(func(o, settings)),
                canConvert);
        }

        public static void RegisterFileConverter(
            Func<object, VerifySettings, Task<ConversionResult>> func,
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
            Func<T, VerifySettings, ConversionResult> func)
        {
            Guard.AgainstNull(func, nameof(func));
            RegisterFileConverter<T>(
                toExtension,
                (o, settings) => Task.FromResult(func(o, settings)));
        }

        public static void RegisterFileConverter<T>(
            string toExtension,
            Func<T, VerifySettings, Task<ConversionResult>> func)
        {
            Guard.AgainstNull(func, nameof(func));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            var converter = new TypeConverter(
                toExtension,
                (o, settings) => func((T) o, settings),
                type => typeof(T).IsAssignableFrom(type));
            typedConverters.Add(converter);
        }

        public static void RegisterFileConverter(
            string toExtension,
            Func<object, VerifySettings, ConversionResult> func,
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
            Func<object, VerifySettings, Task<ConversionResult>> func,
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