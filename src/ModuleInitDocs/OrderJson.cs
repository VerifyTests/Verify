public class OrderJson
{
    #region OrderJsonObjects

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.SortJsonObjects();
    }

    #endregion
}