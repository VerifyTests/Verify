using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static ConcurrentBag<Appender> appenders = new ConcurrentBag<Appender>();

        internal static IEnumerable<ConversionStream> GetContextConverters(VerifySettings settings)
        {
            foreach (var appender in appenders)
            {
                var stream = appender(settings);
                if (stream != null)
                {
                    yield return (ConversionStream)stream;
                }
            }
        }

        public static void RegisterAppender(Appender appender)
        {
            Guard.AgainstNull(appender, nameof(appender));
            appenders.Add(appender);
        }
    }
}