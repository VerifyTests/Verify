[TestClass]
[UsesVerify]
public partial class Tests
{
    [TestMethod]
    public Task Test()
    {
        VerifierSettings.Reset();
        DerivePathInfo(
            (sourceFile, projectDirectory, methodName, typeName) =>
            {
                Assert.IsTrue(File.Exists(sourceFile));
                Assert.IsTrue(Directory.Exists(projectDirectory));
                Assert.IsNotNull(methodName);
                Assert.IsNotNull(typeName);
                Assert.EndsWith("Verify.MSTest.DerivePaths.Tests/Tests.cs", sourceFile.Replace(@"\", "/"));
                Assert.EndsWith("Verify.MSTest.DerivePaths.Tests/", projectDirectory.Replace(@"\", "/"));
                return new("CustomDir", "CustomTypeName", "CustomMethodName");
            });
        return Verify("Value");
    }

    [TestMethod]
    public Task ReturnNulls()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null));
        return Verify("Value");
    }

    [TestMethod]
    public Task ProjectRelativeDirectory()
    {
        VerifierSettings.Reset();
        UseProjectRelativeDirectory("Relative");
        return Verify("Value");
    }

    [TestMethod]
    public Task SourceFileRelativeDirectory()
    {
        VerifierSettings.Reset();
        UseSourceFileRelativeDirectory("Relative");
        return Verify("Value");
    }

    [TestMethod]
    public Task InvalidMethod()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null, null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        return Assert.ThrowsExactlyAsync<ArgumentException>(() => Verify("Value"));
    }

    [TestMethod]
    public Task InvalidType()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        return Assert.ThrowsExactlyAsync<ArgumentException>(() => Verify("Value"));
    }

    [TestMethod]
    public Task InvalidDirectory()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(Path
            .GetInvalidPathChars()
            .First()
            .ToString()));
        return Assert.ThrowsExactlyAsync<ArgumentException>(() => Verify("Value"));
    }
}