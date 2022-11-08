[UsesVerify]
public class Tests
{
    [Theory]
    [InlineData("Value1")]
    public Task MissingParameter(string arg) =>
        Verify("Foo");

    [Theory]
    [MemberData(nameof(GetData))]
    public Task UseFileNameWithParam(string arg) =>
        Verify(arg)
            .UseFileName("UseFileNameWithParam");

    public static IEnumerable<object[]> GetData() =>
        new[]
        {
            new object[]
            {
                "Value1"
            }
        };

    #region ExplicitTargetsXunit

    [Fact]
    public Task WithTargets() =>
        Verify(
            new
            {
                Property = "Value"
            },
            new[]
            {
                new Target(
                    extension: "txt",
                    data: "Raw target value",
                    name: "targetName")
            });

    #endregion

    [ModuleInitializer]
    public static void InitWithTargetsAndConverter() =>
        VerifierSettings.RegisterFileConverter(
            "WithTargetsAndConverter",
            (_, _) =>
                new(
                    "theInfo",
                    new List<Target>
                    {
                        new("txt", "text from converter")
                    }));

    [Fact]
    public Task WithTargetsAndConverter() =>
        Verify(
            new
            {
                Property = "Value"
            },
            new[]
            {
                new Target(
                    extension: "WithTargetsAndConverter",
                    data: new MemoryStream(),
                    name: "targetName")
            });

    [Fact]
    public Task EnumerableTargets() =>
        Verify(
            new[]
            {
                new Target(
                    extension: "txt",
                    data: "Raw target value",
                    name: "targetName")
            });

    static string directoryToVerify = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify");

    #region VerifyDirectoryXunit

    [Fact]
    public Task WithDirectory() =>
        VerifyDirectory(directoryToVerify);

    #endregion

    #region VerifyDirectoryWithInfo

    [Fact]
    public Task VerifyDirectoryWithInfo() =>
        VerifyDirectory(
            directoryToVerify,
            info: "the info");

    #endregion

    #region VerifyDirectoryWithFileScrubber

    [Fact]
    public Task VerifyDirectoryWithFileScrubber() =>
        VerifyDirectory(
            directoryToVerify,
            fileScrubber: (path, builder) =>
            {
                if (Path.GetFileName(path) == "TextDoc.txt")
                {
                    builder.Clear();
                    builder.Append("New text");
                }
            });

    #endregion

#if !NET48

    #region VerifyDirectoryFilterXunit

    [Fact]
    public Task WithDirectoryFiltered() =>
        VerifyDirectory(
            directoryToVerify,
            include: filePath => filePath.Contains("Doc"),
            pattern: "*.txt",
            options: new()
            {
                RecurseSubdirectories = false
            });

    #endregion

#endif
}