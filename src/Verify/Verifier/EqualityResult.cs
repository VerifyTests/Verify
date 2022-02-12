readonly struct EqualityResult
{
    public Equality Equality { get; }
    public string? Message { get; }

    public EqualityResult(in Equality equality, in string? message, in string? receivedText, in string? verifiedText)
    {
        Equality = equality;
        Message = message;
    }
}