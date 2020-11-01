﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static List<TypeConverter> typedConverters = new List<TypeConverter>();

        internal static bool TryGetTypedConverter<T>(
            T target,
            VerifySettings settings,
            [NotNullWhen(true)] out TypeConverter? converter)
        {
            foreach (var typedConverter in typedConverters
                .Where(_ => _.CanConvert(target!, settings.Context)))
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
                (o, context) => Task.FromResult(conversion(o, context)),
                canConvert);
        }

        public static void RegisterFileConverter<T>(
            AsyncConversion<T> conversion,
            CanConvert<T>? canConvert = null)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            var converter = new TypeConverter(
                (o, context) => conversion((T) o, context),
                DefaultCanConvert(canConvert));
            typedConverters.Add(converter);
        }

        public static void RegisterFileConverter(
            Conversion conversion,
            CanConvert canConvert)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            RegisterFileConverter(
                (o, context) => Task.FromResult(conversion(o, context)),
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

        static CanConvert DefaultCanConvert<T>(CanConvert<T>? canConvert)
        {
            if (canConvert == null)
            {
                return (target, _) => target is T;
            }

            return (target, settings) =>
            {
                if (target is T cast)
                {
                    return canConvert(cast, settings);
                }

                return false;
            };
        }
    }
}