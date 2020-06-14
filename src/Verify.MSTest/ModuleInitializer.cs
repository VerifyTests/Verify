using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyTesting;

static class ModuleInitializer
{
    public static void Initialize()
    {
        InnerVerifier.Init(message => new AssertFailedException(message));
    }
}