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
            value = value.Replace("\r\n", "\n").Replace("\r", "\n");
            if (value.Contains("\n"))
            {
                value = $"\n{value}";
            }
            base.WriteRawValue(value);
            return;
        }
        base.WriteRawValue(value);
    }
}