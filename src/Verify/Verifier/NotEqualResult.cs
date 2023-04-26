readonly struct NotEqualResult
{
    public FilePair File { get; }
    public string? Message { get; }
    public StringBuilder? ReceivedText { get; }
    public StringBuilder? VerifiedText { get; }

    public NotEqualResult(FilePair file, string? message, StringBuilder? receivedText, StringBuilder? verifiedText)
    {
        File = file;
        Message = message;
        ReceivedText = receivedText;
        VerifiedText = verifiedText;
    }
}