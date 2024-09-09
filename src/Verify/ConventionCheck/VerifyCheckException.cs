class VerifyCheckException(string message) :
    Exception(message)
{
    public override string ToString() => Message;

    public override string StackTrace => " ";
}