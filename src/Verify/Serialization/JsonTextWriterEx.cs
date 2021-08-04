using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using VerifyTests;

class JsonTextWriterEx :
    JsonTextWriter
{
    StringBuilder builder;
    public Dictionary<string, object> Context { get; }

    public JsonTextWriterEx(StringWriter writer, StringBuilder builder, Dictionary<string, object> context) :
        base(writer)
    {
        this.builder = builder;
        Context = context;
        if (!VerifierSettings.StrictJson)
        {
            QuoteChar = '\'';
            QuoteName = false;
        }
    }

    public override void WriteValue(string? value)
    {
        if (value is null)
        {
            base.WriteValue(value);
            return;
        }

        value = value.Replace("\r\n", "\n").Replace('\r', '\n');
        if (VerifierSettings.StrictJson)
        {
            base.WriteValue(value);
            return;
        }

        if (value.Contains("\n"))
        {
            base.Flush();
            var builderLength = builder.Length;
            value = $"\n{value}";
            base.WriteRawValue(value);
            base.Flush();
            builder.Remove(builderLength, 1);
        }
        else
        {
            base.WriteRawValue(value);
        }

    }

    public override void WriteValue(byte[]? value)
    {
        if (value is null)
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