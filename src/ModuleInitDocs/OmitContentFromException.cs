public class OmitContentFromException
{
    #region OmitContentFromException

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.OmitContentFromException();
    }

    #endregion
}
