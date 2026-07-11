public class ExcludeTargets
{
    #region StaticExcludeTargets

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.ExcludeTargets("pdf");
    }

    #endregion
}
