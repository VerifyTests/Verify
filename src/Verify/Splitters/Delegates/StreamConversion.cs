namespace VerifyTests;

public delegate Task<ConversionResult> AsyncStreamConversion(string? name, Stream target, VerifySettings settings);

public delegate ConversionResult StreamConversion(string? name, Stream target, VerifySettings settings);