public class UseSsimForPng
{
    #region UseSsimForPng

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.UseSsimForPng();
    }

    #endregion
}
