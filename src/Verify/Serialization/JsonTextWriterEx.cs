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
        if (value != null && value.Contains("\n"))
        {
            base.WriteRawValue($"\n{value}");
            return;
        }
        base.WriteRawValue(value);
    }
}