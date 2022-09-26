readonly struct NewResult
{
    public FilePair File { get; }
    public StringBuilder? ReceivedText { get; }

    public NewResult(in FilePair file, in StringBuilder? receivedText)
    {
        File = file;
        ReceivedText = receivedText;
    }
}