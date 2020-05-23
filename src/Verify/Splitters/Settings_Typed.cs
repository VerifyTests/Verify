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
            T target,
            string? extension,
            [NotNullWhen(true)] out TypeConverter? converter)
        {
            if (extension != null)
            {
                foreach (var typedConverter in typedConverters
                    .Where(_ => _.ToExtension != null &&
                                _.ToExtension == extension &&
                                _.CanConvert(target!)))
                {
                    converter = typedConverter;
                    return true;
                }
            }

            foreach (var typedConverter in typedConverters
                .Where(_ => _.CanConvert(target!)))
            {
                converter = typedConverter;
                return true;
            }

            converter = null;
            return false;
        }

        public static void RegisterFileConverter<T>(
            Conversion<T> conversion,
            CanConvert<T>? canConvert = null)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            RegisterFileConverter(
                (o, settings) => Task.FromResult(conversion(o, settings)),
                canConvert);
        }

        public static void RegisterFileConverter<T>(
            AsyncConversion<T> conversion,
            CanConvert<T>? canConvert = null)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            var converter = new TypeConverter(
                (o, settings) => conversion((T) o, settings),
                DefaultCanConvert(canConvert));
            typedConverters.Add(converter);
        }

        public static void RegisterFileConverter(
            Conversion conversion,
            CanConvert canConvert)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            RegisterFileConverter(
                (o, settings) => Task.FromResult(conversion(o, settings)),
                canConvert);
        }

        public static void RegisterFileConverter(
            AsyncConversion conversion,
            CanConvert canConvert)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            Guard.AgainstNull(canConvert, nameof(canConvert));
            var converter = new TypeConverter(
                conversion,
                canConvert);
            typedConverters.Add(converter);
        }

        public static void RegisterFileConverter<T>(
            string toExtension,
            Conversion<T> conversion,
            CanConvert<T>? canConvert = null)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            RegisterFileConverter(
                toExtension,
                (o, settings) => Task.FromResult(conversion(o, settings)),
                canConvert);
        }

        public static void RegisterFileConverter<T>(
            string toExtension,
            AsyncConversion<T> conversion,
            CanConvert<T>? canConvert = null)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));

            var converter = new TypeConverter(
                toExtension,
                (o, settings) => conversion((T) o, settings),
                DefaultCanConvert(canConvert));
            typedConverters.Add(converter);
        }

        static CanConvert DefaultCanConvert<T>(CanConvert<T>? canConvert)
        {
            if (canConvert == null)
            {
                return target => target is T;
            }

            return target =>
            {
                if (target is T cast)
                {
                    return canConvert(cast);
                }

                return false;
            };
        }

        public static void RegisterFileConverter(
            string toExtension,
            Conversion conversion,
            CanConvert canConvert)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            RegisterFileConverter(
                toExtension,
                (o, settings) => Task.FromResult(conversion(o, settings)),
                canConvert);
        }

        public static void RegisterFileConverter(
            string toExtension,
            AsyncConversion conversion,
            CanConvert canConvert)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            Guard.AgainstNull(canConvert, nameof(canConvert));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            var converter = new TypeConverter(
                toExtension,
                conversion,
                canConvert);
            typedConverters.Add(converter);
        }
    }
}