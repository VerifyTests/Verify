public class UseSsimForPngThreshold
{
    #region UseSsimForPngThreshold

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.UseSsimForPng(threshold: 0.995);
    }

    #endregion
}
