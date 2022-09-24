namespace VerifyTests;

public delegate bool CanConvert<in T>(T target, IReadOnlyDictionary<string, object> context);

public delegate bool CanConvert(object target, IReadOnlyDictionary<string, object> context);