public class AutoVerify
{
    #region StaticAutoVerify

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.AutoVerify();
    }

    #endregion
}