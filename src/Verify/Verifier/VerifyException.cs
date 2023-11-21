class VerifyException(string message) : Exception(message)
{
    public override string StackTrace => "";
}