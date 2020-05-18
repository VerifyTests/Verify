using System.Text;
using Newtonsoft.Json;

static class JsonFormatter
{
    public static StringBuilder AsJson<T>(T input, JsonSerializerSettings settings, bool newLineEscapingDisabled)
    {
        var serializer = JsonSerializer.Create(settings);
        var builder = new StringBuilder();
        using var stringWriter = new StringWriterEx(builder,newLineEscapingDisabled);
        using var writer = new JsonTextWriter(stringWriter)
        {
            QuoteChar = '\'',
            QuoteName = false
        };
        serializer.Serialize(writer, input);
        return builder;
    }
}