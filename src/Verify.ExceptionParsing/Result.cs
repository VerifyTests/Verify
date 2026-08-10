namespace VerifyTests.ExceptionParsing;

public readonly struct Result(
    IList<FilePair> @new,
    IList<FilePair> notEqual,
    IList<string> delete,
    IList<FilePair> equal,
    IList<InlineEntry> inlineNew,
    IList<InlineEntry> inlineNotEqual)
{
    public Result(
        IList<FilePair> @new,
        IList<FilePair> notEqual,
        IList<string> delete,
        IList<FilePair> equal)
        : this(@new, notEqual, delete, equal, [], [])
    {
    }

    public IList<FilePair> New { get; } = @new;
    public IList<FilePair> NotEqual { get; } = notEqual;
    public IList<string> Delete { get; } = delete;
    public IList<FilePair> Equal { get; } = equal;
    public IList<InlineEntry> InlineNew { get; } = inlineNew;
    public IList<InlineEntry> InlineNotEqual { get; } = inlineNotEqual;
}
