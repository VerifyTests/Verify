namespace VerifyTests;

public class VerifyResult
{
    public VerifyResult(IReadOnlyList<string> files)
    {
        Files = files;
    }

    public IReadOnlyList<string> Files { get; }
}