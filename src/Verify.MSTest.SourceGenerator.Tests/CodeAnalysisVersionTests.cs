using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

public class CodeAnalysisVersionTests(ITestOutputHelper output) : TestBase(output)
{
    // prevent a CS9057 https://github.com/VerifyTests/Verify/issues/1255
    [Fact]
    public void AssertVersion()
    {
        var assemblyName = typeof(CSharpExtensions).Assembly.GetName();
        Assert.Equal(new(4, 9, 0, 0), assemblyName.Version);
    }
}
