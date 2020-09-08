using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static ConcurrentBag<JsonAppender> jsonAppenders = new ConcurrentBag<JsonAppender>();

        internal static IEnumerable<ToAppend> GetJsonAppenders(VerifySettings settings)
        {
            foreach (var appender in jsonAppenders)
            {
                var data = appender(settings);
                if (data != null)
                {
                    yield return data.Value;
                }
            }
        }

        public static void RegisterJsonAppender(JsonAppender appender)
        {
            Guard.AgainstNull(appender, nameof(appender));
            jsonAppenders.Add(appender);
        }
    }
}