using System;
using System.Collections.Generic;
using System.Globalization;
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

    public override void WriteValue(Uri? value)
    {
        if (value == null)
        {
            WriteNull();
        }
        else
        {
            WriteValue(value.OriginalString);
        }
    }

    public override void WriteValue(byte[]? value)
    {
        if (value == null)
        {
            WriteNull();
        }
        else
        {
            WriteValue(Convert.ToBase64String(value));
        }
    }

    public override void WriteValue(DateTimeOffset value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            WriteValue(value.ToString("yyyy-MM-ddK", CultureInfo.InvariantCulture));
        }
        else
        {
            WriteValue(value.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture));
        }
    }

    public override void WriteValue(DateTime value)
    {
        if (value.TimeOfDay == TimeSpan.Zero)
        {
            WriteValue(value.ToString("yyyy-MM-ddK", CultureInfo.InvariantCulture));
        }
        else
        {
            WriteValue(value.ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFFFK", CultureInfo.InvariantCulture));
        }
    }

    public override void WriteValue(TimeSpan value)
    {
        WriteValue(value.ToString());
    }
}