class VerifyException(string message, IReadOnlyCollection<FilePair> notEqualPairs) : Exception(message)
{
    public IReadOnlyCollection<FilePair> NotEqualPairs => notEqualPairs;
    public override string StackTrace => "";
}