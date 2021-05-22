using System;
using System.IO;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class Tests
{
    [Fact]
    public Task Test()
    {
        VerifierSettings.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) =>
            {
                Assert.True(File.Exists(sourceFile));
                Assert.True(Directory.Exists(projectDirectory));
                Assert.NotNull(method);
                Assert.NotNull(type);
                Assert.EndsWith("Verify.DerivePaths.Tests/Tests.cs", sourceFile.Replace(@"\", "/"));
                Assert.EndsWith("Verify.DerivePaths.Tests/", projectDirectory.Replace(@"\", "/"));
                return new("CustomDir", "CustomTypeName", "CustomMethodName");
            });
        return Verifier.Verify("Value");
    }

    [Fact]
    public Task ReturnNulls()
    {
        VerifierSettings.DerivePathInfo((_, _, _, _) => new PathInfo(null));
        return Verifier.Verify("Value");
    }

    [Fact]
    public Task InvalidMethod()
    {
        VerifierSettings.DerivePathInfo((_, _, _, _) => new PathInfo(null, null, "|"));
        return Assert.ThrowsAsync<ArgumentException>(() =>  Verifier.Verify("Value"));
    }

    [Fact]
    public Task InvalidType()
    {
        VerifierSettings.DerivePathInfo((_, _, _, _) => new PathInfo(null, "|"));
        return Assert.ThrowsAsync<ArgumentException>(() =>  Verifier.Verify("Value"));
    }

    [Fact]
    public Task InvalidDirectory()
    {
        VerifierSettings.DerivePathInfo((_, _, _, _) => new PathInfo("|"));
        return Assert.ThrowsAsync<ArgumentException>(() =>  Verifier.Verify("Value"));
    }
}