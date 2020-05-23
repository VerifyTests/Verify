namespace Verify
{
    public delegate ConversionResult ObjectConversion<in T>(T target, VerifySettings settings);
}