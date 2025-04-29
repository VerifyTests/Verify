public class VerifyDirectoryTests
{
    static string directoryPathToVerify = Path.Combine(AttributeReader.GetSolutionDirectory(), "ToVerify");
#if NET8_0_OR_GREATER

#region AddTextFileConvention
    [ModuleInitializer]
    public static void InitTextFileConvention() =>
        FileExtensions.AddTextFileConvention(
            path =>
            {
                var name = Path.GetFileName(path);
                return name.Equals("TextDocWithoutExtension", StringComparison.OrdinalIgnoreCase);
            });
#endregion

#endif

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
        VerifierSettings.RegisterStreamConverter(
            "VerifyDirectoryTests",
            (_, _, _) =>
                new(
                    "info from FileConverter",
                    [new("txt", "text from FileConverter")]));
}