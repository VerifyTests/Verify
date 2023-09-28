readonly struct NewResult(FilePair file, StringBuilder? receivedText)
{
    public FilePair File { get; } = file;
    public StringBuilder? ReceivedText { get; } = receivedText;
}