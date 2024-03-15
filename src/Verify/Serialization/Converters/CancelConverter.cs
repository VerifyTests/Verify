class CancelConverter :
    WriteOnlyJsonConverter<Cancel>
{
    public override void Write(VerifyJsonWriter writer, Cancel value)
    {
        writer.WriteStartObject();

        writer.WriteMember(value, value.IsCancellationRequested, "IsCancellationRequested");
        writer.WriteMember(value, value.CanBeCanceled, "Token");
        try
        {
            writer.WriteMember(value, value.WaitHandle, "WaitHandle");
        }
        catch (ObjectDisposedException)
        {
        }

        writer.WriteEndObject();
    }
}