class AggregateExceptionConverter :
    WriteOnlyJsonConverter<AggregateException>
{
    public override void Write(VerifyJsonWriter writer, AggregateException exception)
    {
        writer.WriteStartObject();

        writer.WriteProperty(exception, exception.GetType(), "Type");
        if (exception.InnerExceptions.Count == 1)
        {
            writer.WriteProperty(exception, exception.InnerException, "InnerException");
        }
        else
        {
            writer.WriteProperty(exception, exception.InnerExceptions, "InnerExceptions");
        }

        writer.WriteProperty(exception, exception.StackTrace, "StackTrace");

        writer.WriteEndObject();
    }
}