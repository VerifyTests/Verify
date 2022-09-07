public static class ModuleInit
{
    #region UseStrictJson

    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.UseStrictJson();

        #endregion
        DerivePathInfo(
            (_, _, typeName, methodName) => new(AttributeReader.GetProjectDirectory(), typeName: typeName, methodName));
    }
}