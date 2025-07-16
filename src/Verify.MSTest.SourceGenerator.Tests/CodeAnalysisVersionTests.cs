using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

public class CodeAnalysisVersionTests(ITestOutputHelper output) : TestBase(output)
{
    // prevent a CS9057 https://github.com/VerifyTests/Verify/issues/1255
    // version compat: https://learn.microsoft.com/en-us/visualstudio/extensibility/roslyn-version-support?view=vs-2022
    [Fact]
    public void AssertVersion()
    {
        var assemblyName = typeof(CSharpExtensions).Assembly.GetName();
        Assert.Equal(new(4, 9, 0, 0), assemblyName.Version);
    }
}
