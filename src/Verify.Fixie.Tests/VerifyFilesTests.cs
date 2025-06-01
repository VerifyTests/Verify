public class VerifyFilesTests
{
    public Task Run() =>
        VerifyFiles(["File1.txt", "File2.txt"]);

    public Task WithInfo() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            info: new
            {
                Key = "Value"
            });

    public Task WithFileScrubber() =>
        VerifyFiles(
            ["File1.txt", "File2.txt"],
            fileScrubber: (_, builder) =>
            {
                builder.Clear();
                builder.Append("New");
            });
}