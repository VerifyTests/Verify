using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace VerifyXunit
{
    public static class JsonFormatter
    {
        public static string AsJson(object target, JsonSerializerSettings? jsonSerializerSettings = null)
        {
            var serializer = GetSerializer(jsonSerializerSettings);
            var builder = new StringBuilder();
            using var stringWriter = new StringWriter(builder);
            using var writer = new JsonTextWriter(stringWriter)
            {
                QuoteChar = '\'',
                QuoteName = false
            };
            serializer.Serialize(writer, target);
            builder.Replace(@"\\", @"\");
            return builder.ToString();
        }

        static JsonSerializer GetSerializer(JsonSerializerSettings? jsonSerializerSettings)
        {
            if (jsonSerializerSettings == null)
            {
                return JsonSerializer.Create(SerializerBuilder.BuildSettings());
            }

            return JsonSerializer.Create(jsonSerializerSettings);
        }
    }
}