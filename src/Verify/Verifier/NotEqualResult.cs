readonly struct NotEqualResult(
    FilePair file,
    string? message,
    StringBuilder? receivedText,
    string? verifiedText)
{
    public FilePair File { get; } = file;
    public string? Message { get; } = message;
    public StringBuilder? ReceivedText { get; } = receivedText;
    public string? VerifiedText { get; } = verifiedText;
}
