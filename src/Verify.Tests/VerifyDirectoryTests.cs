[UsesVerify]
public class VerifyDirectoryTests
{
    static string directoryPathToVerify = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify");

    [Fact]
    public Task WithDirectory() =>
        VerifyDirectory(directoryPathToVerify);

    [Fact]
    public Task WithDirectoryWithInfo() =>
        VerifyDirectory(
            directoryPathToVerify,
            info: "the Info");

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.RegisterFileConverter(
            "VerifyDirectoryTests",
            (_, _) =>
                new(
                    "info from FileConverter",
                    new List<Target>
                    {
                        new("txt", "text from FileConverter")
                    }));
}