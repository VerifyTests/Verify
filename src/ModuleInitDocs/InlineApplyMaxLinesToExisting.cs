public class InlineApplyMaxLinesToExisting
{
    #region StaticInlineApplyMaxLinesToExisting

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.Inline(
                maxLines: 30,
                applyMaxLinesToExisting: true);
    }

    #endregion
}
