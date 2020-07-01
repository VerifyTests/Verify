namespace VerifyTests
{
    public delegate bool CanConvert<in T>(T target, VerifySettings settings);
    public delegate bool CanConvert(object target, VerifySettings settings);
}