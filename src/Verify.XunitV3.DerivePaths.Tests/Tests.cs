public class Tests
{
    [Fact]
    public Task Test()
    {
        VerifierSettings.Reset();
        DerivePathInfo(
            (sourceFile, projectDirectory, methodName, typeName) =>
            {
                Assert.True(File.Exists(sourceFile));
                Assert.True(Directory.Exists(projectDirectory));
                Assert.NotNull(methodName);
                Assert.NotNull(typeName);
                Assert.EndsWith("Verify.XunitV3.DerivePaths.Tests/Tests.cs", sourceFile.Replace(@"\", "/"));
                Assert.EndsWith("Verify.XunitV3.DerivePaths.Tests/", projectDirectory.Replace(@"\", "/"));
                return new("CustomDir", "CustomTypeName", "CustomMethodName");
            });
        return Verify("Value");
    }

    [Fact]
    public Task ReturnNulls()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null));
        return Verify("Value");
    }

    [Fact]
    public Task ProjectRelativeDirectory()
    {
        VerifierSettings.Reset();
        UseProjectRelativeDirectory("Relative");
        return Verify("Value");
    }

    [Fact]
    public Task SourceFileRelativeDirectory()
    {
        VerifierSettings.Reset();
        UseSourceFileRelativeDirectory("Relative");
        return Verify("Value");
    }

    [Fact]
    public Task InvalidMethod()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null, null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        return Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Fact]
    public Task InvalidType()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        return Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Fact]
    public Task InvalidDirectory()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(Path
            .GetInvalidPathChars()
            .First()
            .ToString()));
        return Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }
}