public static class ModuleInit
{
    #region UseStrictJson

    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.UseStrictJson();

        #endregion
        VerifierSettings.DerivePathInfo(
            (_, _, methodName, typeName) => new(AttributeReader.GetProjectDirectory(), typeName, methodName));
    }

}