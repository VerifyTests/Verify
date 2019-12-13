namespace Verify
{
    static class VerifySettingsEx
    {
        public static VerifySettings OrDefault(this VerifySettings? settings)
        {
            if (settings == null)
            {
                return VerifySettings.Default;
            }

            return settings;
        }
    }
}