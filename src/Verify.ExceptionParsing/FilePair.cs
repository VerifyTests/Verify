namespace VerifyTests.ExceptionParsing;

[DebuggerDisplay("Received = {Received} | Verified = {Verified}")]
public readonly struct FilePair
{
    public string Received { get; }
    public string Verified { get; }

    public FilePair(string received, string verified)
    {
        Received = received;
        Verified = verified;
    }
}