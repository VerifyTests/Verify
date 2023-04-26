readonly struct NewResult
{
    public FilePair File { get; }
    public StringBuilder? ReceivedText { get; }

    public NewResult(FilePair file, StringBuilder? receivedText)
    {
        File = file;
        ReceivedText = receivedText;
    }
}