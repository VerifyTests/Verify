public static class ModuleInit
{
    #region UseStrictJson

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.UseStrictJson();

    #endregion

    [ModuleInitializer]
    public static void InitDerivePathInfo() =>
        DerivePathInfo(
            (_, _, type, method) => new(AttributeReader.GetProjectDirectory(), typeName: type.Name, method.Name));
}