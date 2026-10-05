public class PagedDocuments
{
    #region StaticPagedDocuments

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifierSettings.PageText(PageTextPlacement.PerPage);
            VerifierSettings.PagesToInclude(10);
            VerifierSettings.ExcludeDerivedTargets("png");
        }
    }

    #endregion
}
