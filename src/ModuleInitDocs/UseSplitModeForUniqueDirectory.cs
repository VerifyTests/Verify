public class UseSplitModeForUniqueDirectory
{
    #region UseSplitModeForUniqueDirectory

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.UseSplitModeForUniqueDirectory();
    }

    #endregion
}