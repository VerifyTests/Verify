class FilePair
{
    public string Extension { get; }
    public string Received { get; }
    public string Verified { get; }

    public FilePair(string extension, string received, string verified)
    {
        Extension = extension;
        Received = received;
        Verified = verified;
    }
}