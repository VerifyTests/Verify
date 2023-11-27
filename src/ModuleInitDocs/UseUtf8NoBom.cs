public class UseUtf8NoBom
{
    #region UseUtf8NoBom

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.UseUtf8NoBom();
    }

    #endregion
}