namespace Verify
{
    public delegate ConversionResult InstanceConversion<in T>(T target, VerifySettings settings);
    public delegate ConversionResult InstanceConversion(object target, VerifySettings settings);
}