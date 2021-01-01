using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class StaticSettings
{
    [Fact]
    public Task Test()
    {
        return Verifier.Verify("String to verify");
    }
}

public static class StaticSettingsUsage
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.AddScrubber(_ => _.Replace("String to verify", "new value"));
    }
}

//Only required if using a legacy version of .net
#if(!NET5_0)
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class ModuleInitializerAttribute :
        Attribute
    {
    }
}
#endif