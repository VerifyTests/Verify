public class UseTextDiffFormat
{
    #region UseTextDiffFormat

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.UseTextDiffFormat(DiffEngine.TextDiffFormat.Full);
    }

    #endregion
}
