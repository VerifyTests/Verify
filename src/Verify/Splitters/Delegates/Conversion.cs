namespace VerifyTests;

public delegate ConversionResult Conversion<in T>(T target, IVerifySettings settings);

public delegate ConversionResult Conversion(object target, IVerifySettings settings);