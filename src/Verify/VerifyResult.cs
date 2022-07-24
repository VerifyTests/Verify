namespace VerifyTests;

public class VerifyResult
{
    public VerifyResult(IReadOnlyList<FilePair> files) =>
        Files = files;

    public IReadOnlyList<FilePair> Files { get; }
}