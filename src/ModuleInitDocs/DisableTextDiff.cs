public class DisableTextDiff
{
    #region DisableTextDiff

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.DisableTextDiff();
    }

    #endregion
}
