using System.Reflection;
using VerifyXunit;

static class ModuleInitializer
{
    public static void Initialize()
    {
        Verifier.SetTestAssembly(Assembly.GetExecutingAssembly());
    }
}