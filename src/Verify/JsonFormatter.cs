using System.IO;
using System.Text;
using Newtonsoft.Json;

static class JsonFormatter
{
    public static string AsJson(object target, JsonSerializerSettings settings)
    {
        var serializer = JsonSerializer.Create(settings);
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
}