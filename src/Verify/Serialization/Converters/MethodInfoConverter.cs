﻿using Newtonsoft.Json;
using SimpleInfoName;
using VerifyTests;

class MethodInfoConverter :
    WriteOnlyJsonConverter<MethodInfo>
{
    public override void WriteJson(
        VerifyJsonWriter writer,
        MethodInfo value,
        JsonSerializer serializer)
    {
        writer.WriteValue(value.SimpleName());
    }
}