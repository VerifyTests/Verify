using VerifyTests;

static class VerifySettingsEx
{
    public static VerifySettings OrDefault(this VerifySettings? settings, string sourceFile)
    {
        settings ??= new VerifySettings();
        settings.SourceFile = sourceFile;
        return settings;
    }
}