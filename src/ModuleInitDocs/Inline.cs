public class Inline
{
    #region StaticInline

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.Inline();
    }

    #endregion
}
