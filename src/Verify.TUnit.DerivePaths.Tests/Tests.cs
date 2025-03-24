[NotInParallel("DatabaseTest")]
public class Tests
{
    [Test]
    public Task Test()
    {
        VerifierSettings.Reset();
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
        VerifierSettings.Reset();
        await Assert.That(File.Exists(sourceFile)).IsTrue();
        await Assert.That(Directory.Exists(projectDirectory)).IsTrue();
        await Assert.That(methodName).IsNotNull();
        await Assert.That(typeName).IsNotNull();
    }

    [Test]
    public Task ReturnNulls()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null));
        return Verify("Value");
    }

    [Test]
    public Task ProjectRelativeDirectory()
    {
        VerifierSettings.Reset();
        UseProjectRelativeDirectory("Relative");
        return Verify("Value");
    }

    [Test]
    public Task SourceFileRelativeDirectory()
    {
        VerifierSettings.Reset();
        UseSourceFileRelativeDirectory("Relative");
        return Verify("Value");
    }

    [Test]
    public async Task InvalidMethod()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null, null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        await Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Test]
    public async Task InvalidType()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        await Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }

    [Test]
    public async Task InvalidDirectory()
    {
        VerifierSettings.Reset();
        DerivePathInfo((_, _, _, _) => new(Path
            .GetInvalidPathChars()
            .First()
            .ToString()));
        await Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    }
}