namespace VerifyTests
{
    public delegate ConversionResult Conversion<in T>(T target, VerifySettings settings);
    public delegate ConversionResult Conversion(object target, VerifySettings settings);
}