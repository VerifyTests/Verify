using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyTests;

static class ModuleInitializer
{
    public static void Initialize()
    {
        InnerVerifier.Init(message => new AssertFailedException(message));
    }
}