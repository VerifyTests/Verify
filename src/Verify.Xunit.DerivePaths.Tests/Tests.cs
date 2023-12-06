[UsesVerify]
public class Tests
{
    [Fact]
    public Task Test()
    {
        DerivePathInfo(
            (sourceFile, projectDirectory, methodName, typeName) =>
            {
                Assert.True(File.Exists(sourceFile));
                Assert.True(Directory.Exists(projectDirectory));
                Assert.NotNull(methodName);
                Assert.NotNull(typeName);
                Assert.EndsWith("Verify.Xunit.DerivePaths.Tests/Tests.cs", sourceFile.Replace(@"\", "/"));
                Assert.EndsWith("Verify.Xunit.DerivePaths.Tests/", projectDirectory.Replace(@"\", "/"));
                return new("CustomDir", "CustomTypeName", "CustomMethodName");
            });
        return Verify("Value");
    }

    [Fact]
    public Task ReturnNulls()
    {
        DerivePathInfo((_, _, _, _) => new(null));
        return Verify("Value");
    }

    [Fact]
    public Task ProjectRelativeDirectory()
    {
        UseProjectRelativeDirectory("Relative");
        return Verify("Value");
    }

    [Fact]
    public Task InvalidMethod()
    {
        DerivePathInfo((_, _, _, _) => new(null, null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        return Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Fact]
    public Task InvalidType()
    {
        DerivePathInfo((_, _, _, _) => new(null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        return Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Fact]
    public Task InvalidDirectory()
    {
        DerivePathInfo((_, _, _, _) => new(Path
            .GetInvalidPathChars()
            .First()
            .ToString()));
        return Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }
}