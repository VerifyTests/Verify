using VerifyTests;

static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        var assembly = Assembly.GetEntryAssembly()!;
        TargetAssembly.Assign(assembly);
    }
}
#if(!NET5_0_OR_GREATER)
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    sealed class ModuleInitializerAttribute :
        Attribute
    {
    }
}
#endif