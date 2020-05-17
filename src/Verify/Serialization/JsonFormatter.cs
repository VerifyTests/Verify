using System.IO;
using System.Text;
using Newtonsoft.Json;

static class JsonFormatter
{
    public static StringBuilder AsJson<T>(T input, JsonSerializerSettings settings)
    {
        var serializer = JsonSerializer.Create(settings);
        var builder = new StringBuilder();
        using var stringWriter = new StringWriter(builder)
        {
            NewLine = "\n"
        };
        using var writer = new JsonTextWriter(stringWriter)
        {
            QuoteChar = '\'',
            QuoteName = false
        };
        serializer.Serialize(writer, input);
        builder.Replace(@"\\", @"\");
        return builder;
    }
}