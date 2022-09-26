readonly struct EqualityResult
{
    public Equality Equality { get; }
    public string? Message { get; }
    public StringBuilder? ReceivedText { get; }
    public StringBuilder? VerifiedText { get; }

    public EqualityResult(in Equality equality, in string? message, in StringBuilder? receivedText, in StringBuilder? verifiedText)
    {
        Equality = equality;
        Message = message;
        ReceivedText = receivedText;
        VerifiedText = verifiedText;
    }
}