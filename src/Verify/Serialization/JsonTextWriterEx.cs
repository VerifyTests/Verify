using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class JsonTextWriterEx :
    JsonTextWriter
{
    public Dictionary<string, object> Context { get; }

    public JsonTextWriterEx(TextWriter writer, Dictionary<string, object> context) :
        base(writer)
    {
        Context = context;
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