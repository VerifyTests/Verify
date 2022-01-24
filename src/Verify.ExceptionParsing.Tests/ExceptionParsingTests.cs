using VerifyTests.ExceptionParsing;
using FilePair = VerifyTests.FilePair;

[UsesVerify]
public class ExceptionParsingTests
{
    static string projectDirectory = AttributeReader.GetProjectDirectory();
    string fakeFilePrefix = Path.Combine(projectDirectory, "ExceptionParsingTests.Fake");
    string fakeReceivedTextFile = Path.Combine(projectDirectory, "ExceptionParsingTests.Fake.recevied.txt");
    string fakeReceivedBinFile = Path.Combine(projectDirectory, "ExceptionParsingTests.Fake.recevied.bin");

    [Fact]
    public Task Error_EmptyList()
    {
        return Throws(() => Parser.Parse(new[] {Environment.NewLine}))
            .IgnoreStackTrack();
    }

    [Fact]
    public Task Error_EmptyDirectory()
    {
        return Throws(() => Parser.Parse(new[] {"Directory: "}))
            .IgnoreStackTrack();
    }

    [Fact]
    public Task Empty()
    {
        var equal = new List<FilePair>();
        var @new = new List<FilePair>();
        var notEqual = new List<(FilePair filePair, string? message)>();
        var delete = new List<string>();

        return ParseVerify(@new, notEqual, delete, equal);
    }

    [Fact]
    public Task WithMessage()
    {
        var equal = new List<FilePair>();
        var @new = new List<FilePair>();
        var notEqual = new List<(FilePair filePair, string? message)>
        {
            new(new("txt", fakeFilePrefix), "TheMessage"),
            new(new("bin", fakeFilePrefix), "TheMessage")
        };
        var delete = new List<string>();

        return ParseVerify(@new, notEqual, delete, equal);
    }

    [Fact]
    public Task SingleEqual()
    {
        var equal = new List<FilePair>
        {
            new("txt", fakeFilePrefix)
        };
        var @new = new List<FilePair>();
        var notEqual = new List<(FilePair filePair, string? message)>();
        var delete = new List<string>();

        return ParseVerify(@new, notEqual, delete, equal);
    }

    [Fact]
    public Task SingleNew()
    {
        var equal = new List<FilePair>();
        var @new = new List<FilePair>
        {
            new("txt", fakeFilePrefix)
        };
        var notEqual = new List<(FilePair filePair, string? message)>();
        var delete = new List<string>();

        return ParseVerify(@new, notEqual, delete, equal);
    }

    [Fact]
    public Task SingleNotEqual()
    {
        var equal = new List<FilePair>();
        var @new = new List<FilePair>();
        var notEqual = new List<(FilePair filePair, string? message)>
        {
            new(new("txt", fakeFilePrefix), null)
        };
        var delete = new List<string>();

        return ParseVerify(@new, notEqual, delete, equal);
    }

    [Fact]
    public Task MultipleItem()
    {
        var equal = new List<FilePair>
        {
            new("txt", fakeFilePrefix),
            new("bin", fakeFilePrefix)
        };
        var @new = new List<FilePair>
        {
            new("txt", fakeFilePrefix),
            new("bin", fakeFilePrefix)
        };
        var notEqual = new List<(FilePair filePair, string? message)>
        {
            new(new("txt", fakeFilePrefix), null),
            new(new("bin", fakeFilePrefix), null)
        };
        var delete = new List<string>
        {
            fakeReceivedTextFile,
            fakeReceivedBinFile
        };

        return ParseVerify(@new, notEqual, delete, equal);
    }

    [Fact]
    public Task SingleItem()
    {
        var equal = new List<FilePair>
        {
            new("txt", fakeFilePrefix)
        };
        var @new = new List<FilePair>
        {
            new("txt", fakeFilePrefix)
        };
        var notEqual = new List<(FilePair filePair, string? message)>
        {
            new(new("txt", fakeFilePrefix), null)
        };
        var delete = new List<string>
        {
            fakeReceivedTextFile
        };

        return ParseVerify(@new, notEqual, delete, equal);
    }

    [Fact]
    public Task SingleDelete()
    {
        var equal = new List<FilePair>();
        var @new = new List<FilePair>();
        var notEqual = new List<(FilePair filePair, string? message)>();
        var delete = new List<string>
        {
            fakeReceivedTextFile
        };

        return ParseVerify(@new, notEqual, delete, equal);
    }

    static async Task ParseVerify(
        List<FilePair> @new,
        List<(FilePair filePair, string? message)> notEqual,
        List<string> delete,
        List<FilePair> equal,
        [CallerFilePath] string sourceFile = "")
    {
        var exceptionMessage = await VerifyExceptionMessageBuilder.Build(projectDirectory, @new, notEqual, delete, equal);

        var lines = exceptionMessage.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        var result = Parser.Parse(lines);
        await Verify(
            new
            {
                exceptionMessage,
                result
            },
            sourceFile: sourceFile);
    }
}