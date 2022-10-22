namespace VerifyTests;

public delegate Task<ConversionResult> AsyncConversion<in T>(T target, IVerifySettings settings);

public delegate Task<ConversionResult> AsyncConversion(object target, IVerifySettings settings);