﻿#if NET6_0_OR_GREATER
class TimeConverter :
    WriteOnlyJsonConverter<TimeOnly>
{
    public override void Write(VerifyJsonWriter writer, TimeOnly value) =>
        writer.WriteRawValue(value.ToString("h:mm tt", CultureInfo.InvariantCulture));
}
#endif