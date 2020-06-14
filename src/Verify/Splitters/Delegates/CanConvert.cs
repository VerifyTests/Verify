namespace VerifyTesting
{
    public delegate bool CanConvert<in T>(T target);
    public delegate bool CanConvert(object target);
}