public class FixNewlinesOnRead
{
    #region FixNewlinesOnRead

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.FixNewlinesOnRead();
    }

    #endregion
}
