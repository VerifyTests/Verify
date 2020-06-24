using NUnit.Framework;
using VerifyTests;

static class ModuleInitializer
{
    public static void Initialize()
    {
        InnerVerifier.Init(message => new AssertionException(message));
    }
}