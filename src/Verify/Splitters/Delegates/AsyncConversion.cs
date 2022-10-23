namespace VerifyTests;

public delegate Task<ConversionResult> AsyncConversion<in T>(T target, IReadOnlyDictionary<string, object> context);

public delegate Task<ConversionResult> AsyncConversion(object target, IReadOnlyDictionary<string, object> context);