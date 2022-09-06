﻿class ParameterInfoConverter :
    WriteOnlyJsonConverter<ParameterInfo>
{
    public override void Write(VerifyJsonWriter writer, ParameterInfo value) =>
        writer.WriteRawValueIfNoStrict(value.SimpleName());
}