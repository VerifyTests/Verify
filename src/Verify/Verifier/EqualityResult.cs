readonly struct EqualityResult(Equality equality, string? message, StringBuilder? receivedText, StringBuilder? verifiedText)
{
    public Equality Equality { get; } = equality;
    public string? Message { get; } = message;
    public StringBuilder? ReceivedText { get; } = receivedText;
    public StringBuilder? VerifiedText { get; } = verifiedText;
}