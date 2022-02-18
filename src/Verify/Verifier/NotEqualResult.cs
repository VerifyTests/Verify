readonly struct NotEqualResult
{
    public FilePair File { get; }
    public string? Message { get; }
    public string? ReceivedText { get; }
    public string? VerifiedText { get; }

    public NotEqualResult(in FilePair file, in string? message, in string? receivedText, in string? verifiedText)
    {
        File = file;
        Message = message;
        ReceivedText = receivedText;
        VerifiedText = verifiedText;
    }
}