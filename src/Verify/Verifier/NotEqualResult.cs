readonly struct NotEqualResult(
    FilePair file,
    string? message,
    StringBuilder? receivedText,
    StringBuilder? verifiedText)
{
    public FilePair File { get; } = file;
    public string? Message { get; } = message;
    public StringBuilder? ReceivedText { get; } = receivedText;
    public StringBuilder? VerifiedText { get; } = verifiedText;
}