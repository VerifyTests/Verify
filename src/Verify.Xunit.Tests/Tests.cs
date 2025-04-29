// ReSharper disable UnusedParameter.Local

// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident
#pragma warning disable RegisterStreamScrubber
public class Tests
{
    // ReSharper disable once UnusedMember.Local
    static void DerivePathInfo() =>
        Verifier.DerivePathInfo(
            (sourceFile, projectDirectory, type, method) => new(
                directory: Path.Combine(projectDirectory, "Snapshots"),
                typeName: type.Name,
                methodName: method.Name));

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
    [
        [
            "Value1"
        ]
    ];

    [Fact]
    public Task StringTarget() =>
        Verify(new Target("txt", "Value"));

    [Fact]
    public Task WithTargets() =>
        Verify(
            new
            {
                Property = "Value"
            },
            [
                new(
                    extension: "txt",
                    data: "Raw target value",
                    name: "targetName")
            ]);

    [ModuleInitializer]
    public static void InitWithTargetsAndConverter() =>
        VerifierSettings.RegisterFileConverter(
            "WithTargetsAndConverter",
            (_, _) =>
                new(
                    "theInfo",
                    [new("txt", "text from converter")]));

    [Fact]
    public Task WithTargetsAndConverter() =>
        Verify(
            new
            {
                Property = "Value"
            },
            [
                new(
                    extension: "WithTargetsAndConverter",
                    data: new MemoryStream())
            ]);

    [Fact]
    public Task WithTargetsAndConverterAndName() =>
        Verify(
            new
            {
                Property = "Value"
            },
            [
                new(
                    extension: "WithTargetsAndConverter",
                    data: new MemoryStream(),
                    name: "targetName")
            ]);

    [ModuleInitializer]
    public static void InitWithTargetsAndMultipleFromConverter() =>
        VerifierSettings.RegisterFileConverter(
            "WithTargetsAndMultipleFromConverter",
            (_, _) =>
                new(
                    "theInfo",
                    [
                        new("txt", "text from converter"),
                        new("txt", "text from converter and name", "targetName"),
                    ]));

    [Fact]
    public Task WithTargetsAndMultipleFromConverter() =>
        Verify(
            new
            {
                Property = "Value"
            },
            [
                new(
                    extension: "WithTargetsAndMultipleFromConverter",
                    data: new MemoryStream())
            ]);

    [Fact]
    public Task WithTargetsAndMultipleFromConverterAndName() =>
        Verify(
            new
            {
                Property = "Value"
            },
            [
                new(
                    extension: "WithTargetsAndMultipleFromConverter",
                    data: new MemoryStream(),
                    name: "targetName")
            ]);

    [ModuleInitializer]
    public static void InitWithStreamScrubber() =>
        VerifierSettings.RegisterStreamScrubber(
            "WithStreamScrubber",
            (_,_, _) =>
            {
                var stream = new MemoryStream("result"u8.ToArray());
                return Task.FromResult<StreamScrubberResult?>(new StreamScrubberResult(stream)) ;
            });

    [Fact]
    public Task WithTargetAndStreamScrubber() =>
        Verify(
            new
            {
                Property = "Value"
            },
            [
                new(
                    extension: "WithStreamScrubber",
                    data: new MemoryStream("input"u8.ToArray()))
            ]);

    [Fact]
    public Task WithStreamScrubber()
    {
        var stream = new MemoryStream("input"u8.ToArray());
        return Verify(stream, extension: "WithStreamScrubber");
    }

    [Fact]
    public Task EnumerableTargets() =>
        Verify(
        [
            new Target(
                extension: "txt",
                data: "Raw target value",
                name: "targetName")
        ]);

    static string directoryToVerify = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify");

    [Fact]
    public Task WithDirectory() =>
        VerifyDirectory(directoryToVerify);

    [Fact]
    public Task VerifyDirectoryWithInfo() =>
        VerifyDirectory(
            directoryToVerify,
            info: "the info");

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

#if !NET48

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

#endif

    static string zipPath = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify.zip");

    [Fact]
    public Task WithZip() =>
        VerifyZip(zipPath);

    [Fact]
    public Task WithZipBytes() =>
        VerifyZip(File.ReadAllBytes(zipPath));

    [Fact]
    public Task VerifyZipWithInfo() =>
        VerifyZip(
            zipPath,
            info: "the info");

    [Fact]
    public Task VerifyZipWithFileScrubber() =>
        VerifyZip(
            zipPath,
            fileScrubber: (path, builder) =>
            {
                if (Path.GetFileName(path) == "TextDoc.txt")
                {
                    builder.Clear();
                    builder.Append("New text");
                }
            });

    [Fact]
    public Task WithZipFiltered() =>
        VerifyZip(
            zipPath,
            include: filePath => filePath.FullName.Contains("Doc"));
}