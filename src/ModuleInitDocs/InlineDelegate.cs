// ReSharper disable UnusedParameter.Local
public class InlineDelegate
{
    #region StaticInlineDelegate

    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init() =>
            VerifierSettings.Inline(
                (typeName, methodName, sourceFile, extension) => extension == "txt");
    }

    #endregion
}
