namespace VerifyTests.ExceptionParsing;

public readonly struct Result(
    IList<FilePair> @new,
    IList<FilePair> notEqual,
    IList<string> delete,
    IList<FilePair> equal)
{
    public IList<FilePair> New { get; } = @new;
    public IList<FilePair> NotEqual { get; } = notEqual;
    public IList<string> Delete { get; } = delete;
    public IList<FilePair> Equal { get; } = equal;
}