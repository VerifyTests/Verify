readonly struct NewResult
{
    public FilePair File { get; }
    public string? ReceivedText { get; }

    public NewResult(in FilePair file, in string? receivedText)
    {
        File = file;
        ReceivedText = receivedText;
    }
}