using VerifyTests;

static class VerifySettingsEx
{
    public static VerifySettings OrDefault(this VerifySettings? settings, string sourceFile)
    {
        Guard.AgainstNullOrEmpty(sourceFile, nameof(sourceFile));
        settings ??= new VerifySettings();
        settings.SourceFile = sourceFile;
        return settings;
    }
}