public class InlineMaxLines
{
    #region StaticInlineMaxLines

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.Inline(maxLines: 30);
    }

    #endregion
}
