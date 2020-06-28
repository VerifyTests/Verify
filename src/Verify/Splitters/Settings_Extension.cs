using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static Dictionary<string, StreamConverter> extensionConverters = new Dictionary<string, StreamConverter>();

        internal static bool TryGetExtensionConverter(string extension, [NotNullWhen(true)] out StreamConverter? converter)
        {
            return extensionConverters.TryGetValue(extension, out converter);
        }

        public static void RegisterFileConverter(
            string fromExtension,
            Conversion<Stream> conversion)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            RegisterFileConverter(
                fromExtension,
                (stream, settings) => Task.FromResult(conversion(stream, settings)));
        }

        public static void RegisterFileConverter(
            string fromExtension,
            AsyncConversion<Stream> conversion)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            Guard.AgainstBadExtension(fromExtension, nameof(fromExtension));
            var converter = new StreamConverter(
                conversion);
            extensionConverters[fromExtension] = converter;
        }
    }
}