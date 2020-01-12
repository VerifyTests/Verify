using System;
using System.Collections.Generic;
using System.IO;

namespace Verify
{
    public static partial class SharedVerifySettings
    {
        static Dictionary<string, StreamConverter> extensionConverters = new Dictionary<string, StreamConverter>();

        internal static bool TryGetConverter(string extension, out StreamConverter converter)
        {
            return extensionConverters.TryGetValue(extension, out converter);
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
    }
}