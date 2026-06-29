readonly struct EqualityResult(Equality equality, string? message, StringBuilder? receivedText, string? verifiedText)
{
    public Equality Equality { get; } = equality;
    public string? Message { get; } = message;
    public StringBuilder? ReceivedText { get; } = receivedText;
    public string? VerifiedText { get; } = verifiedText;
}
