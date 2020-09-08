using VerifyTests;

static class VerifySettingsEx
{
    public static VerifySettings OrDefault(this VerifySettings? settings)
    {
        if (settings == null)
        {
            return new VerifySettings();
        }

        return settings;
    }
}