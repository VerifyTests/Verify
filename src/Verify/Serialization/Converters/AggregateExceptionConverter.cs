class AggregateExceptionConverter :
    WriteOnlyJsonConverter<AggregateException>
{
    public override void Write(VerifyJsonWriter writer, AggregateException exception)
    {
        writer.WriteStartObject();

        writer.WriteMember(exception, exception.GetType(), "Type");
        if (exception.InnerExceptions.Count == 1)
        {
            writer.WriteMember(exception, exception.InnerException, "InnerException");
        }
        else
        {
            writer.WriteMember(exception, exception.InnerExceptions, "InnerExceptions");
        }

        writer.WriteMember(exception, exception.StackTrace, "StackTrace");

        writer.WriteEndObject();
    }
}