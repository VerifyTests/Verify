// ReSharper disable UnusedParameter.Local
public class AutoVerifyDelegate
{
    #region StaticAutoVerifyDelegate

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.AutoVerify(
                (typeName, methodName, verifiedFile) =>
                    Path.GetExtension(verifiedFile) == "png");
    }

    #endregion
}