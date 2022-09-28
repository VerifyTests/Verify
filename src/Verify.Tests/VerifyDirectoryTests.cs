[UsesVerify]
public class VerifyDirectoryTests
{
    static string directoryPathToVerify = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify");

    [Fact]
    public Task WithDirectory() =>
        VerifyDirectory(directoryPathToVerify);

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.RegisterFileConverter(
            "VerifyDirectoryTests",
            (_, _) =>
                new(
                    "info from FileConverter",
                    new List<Target>
                    {
                        new("txt", "text from FileConverter"),
                    }));
}