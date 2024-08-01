namespace VerifyTests.ExceptionParsing;

[DebuggerDisplay("Received = {Received} | Verified = {Verified}")]
public readonly struct FilePair(string received, string verified)
{
    public string Received { get; } = received;
    public string Verified { get; } = verified;
}