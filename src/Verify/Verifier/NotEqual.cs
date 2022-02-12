readonly struct NotEqual
{
    public FilePair File { get; }
    public string? Message { get; }
    public string? ReceivedText { get; }
    public string? VerifiedText { get; }

    public NotEqual(in FilePair file, in string? message, in string? receivedText, in string? verifiedText)
    {
        File = file;
        Message = message;
        ReceivedText = receivedText;
        VerifiedText = verifiedText;
    }
}