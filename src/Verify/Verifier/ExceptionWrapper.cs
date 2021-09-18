class ExceptionWrapper
{
    public ExceptionWrapper(Exception exception)
    {
        Type = exception.GetType().Name;
        Exception = exception;
    }

    public Exception Exception { get; }

    public string Type { get; }
}