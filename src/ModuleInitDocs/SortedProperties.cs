class SortProperties
{
    #region SortProperties

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.SortPropertiesAlphabetically();
    }

    #endregion
}