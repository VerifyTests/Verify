﻿class FieldInfoConverter :
    WriteOnlyJsonConverter<FieldInfo>
{
    public override void Write(VerifyJsonWriter writer, FieldInfo value) =>
        writer.WriteRawValueIfNoStrict(value.SimpleName());
}