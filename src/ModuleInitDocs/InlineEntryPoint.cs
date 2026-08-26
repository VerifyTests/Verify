public class InlineEntryPoint
{
    #region StaticInlineEntryPoint

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.AddInlineEntryPoint("VerifyDocx");
    }

    #endregion
}
