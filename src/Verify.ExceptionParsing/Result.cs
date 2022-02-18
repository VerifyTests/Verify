namespace VerifyTests.ExceptionParsing;

public readonly struct Result
{
    public IList<FilePair> New { get; }
    public IList<FilePair> NotEqual { get; }
    public IList<string> Delete { get; }
    public IList<FilePair> Equal { get; }

    public Result(
        IList<FilePair> @new,
        IList<FilePair> notEqual,
        IList<string> delete,
        IList<FilePair> equal)
    {
        Delete = delete;
        NotEqual = notEqual;
        New = @new;
        Equal = equal;
    }
}