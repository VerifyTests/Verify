public class SortedJson
{
    #region SortJsonObjects

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.SortJsonObjects();
    }

    #endregion
}