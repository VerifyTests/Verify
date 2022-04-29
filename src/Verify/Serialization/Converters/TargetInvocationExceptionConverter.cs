class TargetInvocationExceptionConverter :
    WriteOnlyJsonConverter<TargetInvocationException>
{
    public override void Write(VerifyJsonWriter writer, TargetInvocationException exception) =>
        writer.Serialize(exception.InnerException!);
}