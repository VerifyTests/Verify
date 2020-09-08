using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static ConcurrentBag<FileAppender> fileAppenders = new ConcurrentBag<FileAppender>();

        internal static IEnumerable<ConversionStream> GetFileAppenders(VerifySettings settings)
        {
            foreach (var appender in fileAppenders)
            {
                var stream = appender(settings);
                if (stream != null)
                {
                    yield return (ConversionStream)stream;
                }
            }
        }

        public static void RegisterFileAppender(FileAppender appender)
        {
            Guard.AgainstNull(appender, nameof(appender));
            fileAppenders.Add(appender);
        }
    }
}