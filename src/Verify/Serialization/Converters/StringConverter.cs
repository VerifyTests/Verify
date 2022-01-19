﻿using Newtonsoft.Json;
using VerifyTests;

class StringConverter :
    WriteOnlyJsonConverter<string>
{
    SerializationSettings settings;

    public StringConverter(SerializationSettings settings)
    {
        this.settings = settings;
    }

    public override void Write(
        VerifyJsonWriter writer,
        string value,
        JsonSerializer serializer)
    {
        if (settings.TryConvertString(value, out var result))
        {
            writer.WriteValue(result);
            return;
        }

        writer.WriteValue(value);
    }
}