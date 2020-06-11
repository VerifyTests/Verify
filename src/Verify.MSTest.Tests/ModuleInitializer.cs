using System.Reflection;
using Verify;

static class ModuleInitializer
{
    public static void Initialize()
    {
        SharedVerifySettings.SetTestAssembly(Assembly.GetExecutingAssembly());
    }
}