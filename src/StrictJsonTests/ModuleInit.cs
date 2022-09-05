public static class ModuleInit
{
    #region UseStrictJson

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.UseStrictJson();

    #endregion
}