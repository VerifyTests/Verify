namespace VerifyTests;

public interface IVerifySettings
{
    public IReadOnlyDictionary<string, object> Context { get; }
    bool IsAutoVerify { get; }
}