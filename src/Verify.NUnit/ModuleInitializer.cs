using NUnit.Framework.Internal;
using VerifyTests;

static class ModuleInitializer
{
    public static void Initialize()
    {
        InnerVerifier.Init(message => new NUnitException(message));
    }
}