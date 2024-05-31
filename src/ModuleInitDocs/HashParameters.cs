public class HashParameters
{
    #region StaticHashParameters

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.HashParameters();
    }

    #endregion
}