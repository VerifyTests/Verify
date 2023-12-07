public class Tests
{
    [Tests]
    public static Test Test = Runner.TestCase(
        nameof(Test),
        () =>
        {
            DerivePathInfo(
                (sourceFile, projectDirectory, methodName, typeName) =>
                {
                    Expect.isTrue(File.Exists(sourceFile), "File.Exists(sourceFile)");
                    Expect.isTrue(Directory.Exists(projectDirectory), "Directory.Exists(projectDirectory)");
                    Expect.isNotNull(methodName, "methodName");
                    Expect.isNotNull(typeName, "typeName");
                    Expect.stringEnds("Verify.Xunit.DerivePaths.Tests/Tests.cs", sourceFile.Replace(@"\", "/"), "");
                    Expect.stringEnds("Verify.Xunit.DerivePaths.Tests/", projectDirectory.Replace(@"\", "/"), "");
                    return new("CustomDir", "CustomTypeName", "CustomMethodName");
                });
            return Verify(
                name: nameof(Test),
                target: "Value");
        });

    [Tests]
    public static Test ReturnNulls = Runner.TestCase(
        nameof(ReturnNulls),
        () =>
        {
            DerivePathInfo((_, _, _, _) => new(null));
            return Verify(
                name: nameof(ReturnNulls),
                target: "Value");
        });

    [Tests]
    public static Test ProjectRelativeDirectory = Runner.TestCase(
        nameof(ProjectRelativeDirectory),
        () =>
        {
            UseProjectRelativeDirectory("Relative");
            return Verify(
                name: nameof(ProjectRelativeDirectory),
                target: "Value");
        });

    //
    // [Fact]
    // public Task InvalidMethod()
    // {
    //     DerivePathInfo((_, _, _, _) => new(null, null, Path.GetInvalidFileNameChars().First().ToString()));
    //     return Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    // }
    //
    // [Fact]
    // public Task InvalidType()
    // {
    //     DerivePathInfo((_, _, _, _) => new(null, Path.GetInvalidFileNameChars().First().ToString()));
    //     return Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    // }
    //
    // [Fact]
    // public Task InvalidDirectory()
    // {
    //     DerivePathInfo((_, _, _, _) => new(Path.GetInvalidPathChars().First().ToString()));
    //     return Assert.ThrowsAsync<ArgumentException>(() => Verify("Value"));
    //}
}