public class Tests
{
    [Test]
    public Task Test()
    {
        DerivePathInfo(
            (sourceFile, projectDirectory, methodName, typeName) =>
            {
                Check(sourceFile, projectDirectory, methodName, typeName).GetAwaiter().GetResult();
                return new("CustomDir", "CustomTypeName", "CustomMethodName");
            });
        return Verify("Value");
    }

    static async Task Check(string sourceFile, string projectDirectory, Type methodName, MethodInfo typeName)
    {
        await Assert.That(File.Exists(sourceFile)).IsTrue();
        await Assert.That(Directory.Exists(projectDirectory)).IsTrue();
        await Assert.That(methodName).IsNotNull();
        await Assert.That(typeName).IsNotNull();
    }

    [Test]
    public Task ReturnNulls()
    {
        DerivePathInfo((_, _, _, _) => new(null));
        return Verify("Value");
    }

    [Test]
    public Task ProjectRelativeDirectory()
    {
        UseProjectRelativeDirectory("Relative");
        return Verify("Value");
    }

    [Test]
    public Task SourceFileRelativeDirectory()
    {
        UseSourceFileRelativeDirectory("Relative");
        return Verify("Value");
    }

    [Test]
    public async Task InvalidMethod()
    {
        DerivePathInfo((_, _, _, _) => new(null, null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        await Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Test]
    public async Task InvalidType()
    {
        DerivePathInfo((_, _, _, _) => new(null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        await Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Test]
    public async Task InvalidDirectory()
    {
        DerivePathInfo((_, _, _, _) => new(Path
            .GetInvalidPathChars()
            .First()
            .ToString()));
        await Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }
}