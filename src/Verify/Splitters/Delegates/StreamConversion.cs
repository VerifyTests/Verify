namespace VerifyTests;

public delegate Task<ConversionResult> AsyncStreamConversion(string? name, Stream target, IReadOnlyDictionary<string, object> context);

public delegate ConversionResult StreamConversion(string? name, Stream target, IReadOnlyDictionary<string, object> context);