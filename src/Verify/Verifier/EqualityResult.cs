readonly struct EqualityResult
{
    public Equality Equality { get; }
    public string? Message { get; }
    public string? ReceivedText { get; }
    public string? VerifiedText { get; }

    public EqualityResult(in Equality equality, in string? message, in string? receivedText, in string? verifiedText)
    {
        Equality = equality;
        Message = message;
        ReceivedText = receivedText;
        VerifiedText = verifiedText;
    }
}