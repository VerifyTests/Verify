namespace VerifyTests
{
    public delegate AsStringResult AsString<in T>(T target, IReadOnlyDictionary<string, object> context);
}