[TestClass]
public class Tests :
    VerifyBase
{
    [TestMethod]
    public Task Test()
    {
        DerivePathInfo(
            (sourceFile, projectDirectory, methodName, typeName) =>
            {
                Assert.IsTrue(File.Exists(sourceFile));
                Assert.IsTrue(Directory.Exists(projectDirectory));
                Assert.IsNotNull(methodName);
                Assert.IsNotNull(typeName);
                Assert.IsTrue(sourceFile
                    .Replace(@"\", "/")
                    .EndsWith("Verify.MSTest.DerivePaths.Tests/Tests.cs"));
                Assert.IsTrue(projectDirectory
                    .Replace(@"\", "/")
                    .EndsWith("Verify.MSTest.DerivePaths.Tests/"));
                return new("CustomDir", "CustomTypeName", "CustomMethodName");
            });
        return Verify("Value");
    }

    [TestMethod]
    public Task ReturnNulls()
    {
        DerivePathInfo((_, _, _, _) => new(null));
        return Verify("Value");
    }

    [TestMethod]
    public Task ProjectRelativeDirectory()
    {
        UseProjectRelativeDirectory("Relative");
        return Verify("Value");
    }

    [TestMethod]
    public Task InvalidMethod()
    {
        DerivePathInfo((_, _, _, _) => new(null, null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        return Assert.ThrowsExceptionAsync<ArgumentException>(() => Verify("Value"));
    }

    [TestMethod]
    public Task InvalidType()
    {
        DerivePathInfo((_, _, _, _) => new(null, Path
            .GetInvalidFileNameChars()
            .First()
            .ToString()));
        return Assert.ThrowsExceptionAsync<ArgumentException>(() => Verify("Value"));
    }

    [TestMethod]
    public Task InvalidDirectory()
    {
        DerivePathInfo((_, _, _, _) => new(Path
            .GetInvalidPathChars()
            .First()
            .ToString()));
        return Assert.ThrowsExceptionAsync<ArgumentException>(() => Verify("Value"));
    }
}