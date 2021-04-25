using System.Collections.Generic;
using System.Net.Http;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifyTests;

class HttpContentConverter :
    WriteOnlyJsonConverter<HttpContent>
{
    public override void WriteJson(
        JsonWriter writer,
        HttpContent content,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Headers");
        serializer.Serialize(writer, content.Headers);
        if (content.IsText(out var subType))
        {
            writer.WritePropertyName("Value");
            var result = content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (subType == "json")
            {
                try
                {
                    serializer.Serialize(writer, JToken.Parse(result));
                }
                catch
                {
                    writer.WriteValue(result);
                }
            }
            else if (subType == "xml")
            {
                try
                {
                    serializer.Serialize(writer, XDocument.Parse(result));
                }
                catch
                {
                    writer.WriteValue(result);
                }
            }
            else
            {
                writer.WriteValue(result);
            }
        }

        writer.WriteEndObject();
    }
}