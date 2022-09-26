readonly struct NotEqualResult
{
    public FilePair File { get; }
    public string? Message { get; }
    public StringBuilder? ReceivedText { get; }
    public StringBuilder? VerifiedText { get; }

    public NotEqualResult(in FilePair file, in string? message, in StringBuilder? receivedText, in StringBuilder? verifiedText)
    {
        File = file;
        Message = message;
        ReceivedText = receivedText;
        VerifiedText = verifiedText;
    }
}