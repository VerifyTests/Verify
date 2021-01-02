using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    static Tests()
    {
        VerifierSettings.DeriveTestDirectory((sourceFile, directory) =>
        {
            Assert.True(File.Exists(sourceFile));
            Assert.True(Directory.Exists(directory));
            Assert.EndsWith("Verify.DerivePaths.Tests/Tests.cs", sourceFile.Replace(@"\", "/"));
            Assert.EndsWith("Verify.DerivePaths.Tests/", directory.Replace(@"\", "/"));
            return Path.Combine(directory, "Custom");
        });
    }

    [Fact]
    public async Task Test()
    {
        await Verifier.Verify("Value");
    }
}