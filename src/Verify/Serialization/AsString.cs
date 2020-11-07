namespace VerifyTests
{
    public delegate AsStringResult AsString<in T>(T target, VerifySettings settings);
}