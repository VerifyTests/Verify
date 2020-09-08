using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static ConcurrentBag<ContextConversion> contextConverters = new ConcurrentBag<ContextConversion>();

        internal static bool TryGetContextConverter(VerifySettings settings)
        {
            foreach (var conversion in contextConverters)
            {
                foreach (var VARIABLE in conversion(settings))
                {
                    
                }
            }
            return extensionConverters.TryGetValue(extension, out converter);
        }

        public static void RegisterContextConverter(ContextConversion conversion)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            contextConverters.Add(conversion);
        }
    }
}