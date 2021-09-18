class VerifyException :
    Exception
{
    public VerifyException(string message) :
        base(message)
    {
    }

    public override string StackTrace { get => ""; }
}