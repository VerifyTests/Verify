using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static Dictionary<string, AsyncConversion<Stream>> extensionConverters = new();

        internal static bool TryGetExtensionConverter(string extension, [NotNullWhen(true)] out AsyncConversion<Stream>? converter)
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
                (stream, context) => Task.FromResult(conversion(stream, context)));
        }

        public static void RegisterFileConverter(
            string fromExtension,
            AsyncConversion<Stream> conversion)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            Guard.AgainstBadExtension(fromExtension, nameof(fromExtension));
            extensionConverters[fromExtension] = conversion;
        }
    }
}