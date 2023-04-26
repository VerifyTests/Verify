readonly struct EqualityResult
{
    public Equality Equality { get; }
    public string? Message { get; }
    public StringBuilder? ReceivedText { get; }
    public StringBuilder? VerifiedText { get; }

    public EqualityResult(Equality equality, string? message, StringBuilder? receivedText, StringBuilder? verifiedText)
    {
        Equality = equality;
        Message = message;
        ReceivedText = receivedText;
        VerifiedText = verifiedText;
    }
}