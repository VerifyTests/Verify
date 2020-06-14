using NUnit.Framework.Internal;
using VerifyTesting;

static class ModuleInitializer
{
    public static void Initialize()
    {
        InnerVerifier.Init(message => new NUnitException(message));
    }
}