using System.IO;
using Newtonsoft.Json;

class JsonTextWriterEx :
    JsonTextWriter
{
    public JsonTextWriterEx(TextWriter writer) :
        base(writer)
    {
    }

    public override void WriteValue(string? value)
    {
        if (value != null)
        {
            if (value.Contains("'") && !value.Contains("\""))
            {
                QuoteChar = '"';
                base.WriteValue(value);
                QuoteChar = JsonFormatter.QuoteChar;
                return;
            }
        }

        base.WriteValue(value);
    }
}