namespace VerifyTests;

public interface IVerifySettings
{
    public IReadOnlyDictionary<string, object> Context { get; }

    bool IsAutoVerify { get; }

    internal IReadOnlyCollection<object?>? Parameters{ get; }

    bool IgnoreParametersForVerified { get; }

    bool IsIncludeDateKind { get; }

    bool IsIncludeDateOffset { get; }
    string? FileName { get; }
    string? MethodName { get; }
    string? TypeName { get; }
    string? Directory { get; }
#if DiffEngine
    bool IsDiffEnabled { get; }
#endif
}