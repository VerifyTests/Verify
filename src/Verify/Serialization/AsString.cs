namespace VerifyTests
{
    public delegate string AsString<in T>(T target, VerifySettings settings);
}