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
