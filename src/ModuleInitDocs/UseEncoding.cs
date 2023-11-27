public class UseEncoding
{
    #region UseEncoding

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            var encoding = new UnicodeEncoding(
                bigEndian: false,
                byteOrderMark: true,
                throwOnInvalidBytes: true);
            VerifierSettings.UseEncoding(encoding);
        }
    }

    #endregion
}