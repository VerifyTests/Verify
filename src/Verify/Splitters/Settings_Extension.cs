using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static Dictionary<string, StreamConverter> extensionConverters = new Dictionary<string, StreamConverter>();

        internal static bool TryGetConverter(string extension, out StreamConverter converter)
        {
            return extensionConverters.TryGetValue(extension, out converter);
        }

        public static void RegisterFileConverter(
            string fromExtension,
            string toExtension,
            Conversion<Stream> conversion)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            RegisterFileConverter(
                fromExtension,
                toExtension,
                (stream, settings) => Task.FromResult(conversion(stream, settings)));
        }

        public static void RegisterFileConverter(
            string fromExtension,
            string toExtension,
            AsyncConversion<Stream> conversion)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            Guard.AgainstBadExtension(fromExtension, nameof(fromExtension));
            Guard.AgainstBadExtension(toExtension, nameof(toExtension));
            var converter = new StreamConverter(
                toExtension,
                conversion);
            extensionConverters[fromExtension] = converter;
        }
    }
}