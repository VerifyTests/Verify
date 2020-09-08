using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static ConcurrentBag<ContextConversion> contextConverters = new ConcurrentBag<ContextConversion>();

        internal static IEnumerable<ConversionStream> GetContextConverters(VerifySettings settings)
        {
            foreach (var conversion in contextConverters)
            {
                var stream = conversion(settings);
                if (stream != null)
                {
                    yield return (ConversionStream)stream;
                }
            }
        }

        public static void RegisterContextConverter(ContextConversion conversion)
        {
            Guard.AgainstNull(conversion, nameof(conversion));
            contextConverters.Add(conversion);
        }
    }
}