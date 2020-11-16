using System.Collections.Generic;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static List<FileAppender> fileAppenders = new();

        internal static IEnumerable<ConversionStream> GetFileAppenders(VerifySettings settings)
        {
            foreach (var appender in fileAppenders)
            {
                var stream = appender(settings.Context);
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