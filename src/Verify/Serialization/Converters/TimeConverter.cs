﻿#if NET6_0_OR_GREATER

using Newtonsoft.Json;
using VerifyTests;

class TimeConverter :
    WriteOnlyJsonConverter<TimeOnly>
{
    public override void Write(
        VerifyJsonWriter writer,
        TimeOnly value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString("h:mm tt", serializer.Culture));
    }
}
#endif