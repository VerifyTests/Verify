class OrderProperties
{
    #region OrderProperties

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.SortPropertiesAlphabetically();
    }

    #endregion
}