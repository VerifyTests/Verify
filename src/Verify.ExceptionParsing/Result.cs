namespace VerifyTests.ExceptionParsing;

[DebuggerDisplay("Directory = {Directory}")]
public readonly struct Result
{
    public string Directory { get; }
    public IList<FilePair> New { get; }
    public IList<FilePair> NotEqual { get; }
    public IList<string> Delete { get; }
    public IList<FilePair> Equal { get; }

    public Result(
        string directory,
        IList<FilePair> @new,
        IList<FilePair> notEqual,
        IList<string> delete,
        IList<FilePair> equal)
    {
        Directory = directory;
        Delete = delete;
        NotEqual = notEqual;
        New = @new;
        Equal = equal;
    }
}