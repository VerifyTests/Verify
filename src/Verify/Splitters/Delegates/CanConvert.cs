namespace VerifyTests
{
    public delegate bool CanConvert<in T>(T target, string? extension, IReadOnlyDictionary<string, object> context);
    public delegate bool CanConvert(object target, string? extension, IReadOnlyDictionary<string, object> context);
}