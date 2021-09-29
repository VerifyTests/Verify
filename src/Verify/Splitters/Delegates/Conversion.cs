namespace VerifyTests;

public delegate ConversionResult Conversion<in T>(T target, IReadOnlyDictionary<string, object> context);

public delegate ConversionResult Conversion(object target, IReadOnlyDictionary<string, object> context);