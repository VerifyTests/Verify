public class IgnoreTrailingNewline
{
    #region IgnoreTrailingNewline

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.IgnoreTrailingNewline();
    }

    #endregion
}
