using System.IO;
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
        builder.Replace(@"\\", @"\");
        return builder;
    }
}
class StringWriterEx:StringWriter
{
    bool newLineEscapingDisabled;

    public StringWriterEx(StringBuilder builder, bool newLineEscapingDisabled):
        base(builder)
    {
        this.newLineEscapingDisabled = newLineEscapingDisabled;
        base.NewLine = "\n";
    }

    public override void Write(string value)
    {
        if (newLineEscapingDisabled && value == "\\n")
        {
            base.Write('\n');
        }
        base.Write(value);
    }
}