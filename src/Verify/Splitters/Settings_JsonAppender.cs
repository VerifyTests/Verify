using System.Collections.Generic;

namespace VerifyTests
{
    public static partial class VerifierSettings
    {
        static List<JsonAppender> jsonAppenders = new();

        internal static List<ToAppend> GetJsonAppenders(VerifySettings settings)
        {
            List<ToAppend> list = new();
            foreach (var appender in jsonAppenders)
            {
                var data = appender(settings.Context);
                if (data != null)
                {
                    list.Add(data.Value);
                }
            }

            return list;
        }

        public static void RegisterJsonAppender(JsonAppender appender)
        {
            Guard.AgainstNull(appender, nameof(appender));
            jsonAppenders.Add(appender);
        }
    }
}