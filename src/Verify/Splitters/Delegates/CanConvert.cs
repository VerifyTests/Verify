namespace VerifyTests;

public delegate bool CanConvert<in T>(T target, IVerifySettings settings);

public delegate bool CanConvert(object target, IVerifySettings settings);