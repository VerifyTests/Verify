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
        var notEquals = new List<(FilePair filePair, string? message, NotEqual notEqual)>();
        var delete = new List<string>();

        return ParseVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task WithMessage()
    {
        var equal = new List<FilePair>();
        var @new = new List<FilePair>();
        var notEquals = new List<(FilePair filePair, string? message, NotEqual notEqual)>
        {
            new(new("txt", fakeFilePrefix), "TheMessage", new NotEqual(new("txt", fakeFilePrefix), "TheMessage","receivedText", "verifiedText")),
            new(new("bin", fakeFilePrefix), "TheMessage", new NotEqual(new("txt", fakeFilePrefix), "TheMessage",null,null))
        };
        var delete = new List<string>();

        return ParseVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task SingleEqual()
    {
        var equal = new List<FilePair>
        {
            new("txt", fakeFilePrefix)
        };
        var @new = new List<FilePair>();
        var notEquals = new List<(FilePair filePair, string? message, NotEqual notEqual)>();
        var delete = new List<string>();

        return ParseVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task SingleNew()
    {
        var equal = new List<FilePair>();
        var @new = new List<FilePair>
        {
            new("txt", fakeFilePrefix)
        };
        var notEquals = new List<(FilePair filePair, string? message, NotEqual notEqual)>();
        var delete = new List<string>();

        return ParseVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task SingleNotEqual()
    {
        var equal = new List<FilePair>();
        var @new = new List<FilePair>();
        var notEquals = new List<(FilePair filePair, string? message, NotEqual notEqual)>
        {
            new(new("txt", fakeFilePrefix), null, new NotEqual(new("txt", fakeFilePrefix),null,"receivedText", "verifiedText"))
        };
        var delete = new List<string>();

        return ParseVerify(@new, notEquals, delete, equal);
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
        var notEquals = new List<(FilePair filePair, string? message, NotEqual notEqual)>
        {
            new(new("txt", fakeFilePrefix), null, new NotEqual(new("txt", fakeFilePrefix), null,"receivedText", "verifiedText")),
            new(new("bin", fakeFilePrefix), null, new NotEqual(new("txt", fakeFilePrefix), null,null,null))
        };
        var delete = new List<string>
        {
            fakeReceivedTextFile,
            fakeReceivedBinFile
        };

        return ParseVerify(@new, notEquals, delete, equal);
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
        var notEquals = new List<(FilePair filePair, string? message, NotEqual notEqual)>
        {
            new(new("txt", fakeFilePrefix), null, new NotEqual(new("txt", fakeFilePrefix), null,"receivedText", "verifiedText"))
        };
        var delete = new List<string>
        {
            fakeReceivedTextFile
        };

        return ParseVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task SingleDelete()
    {
        var equal = new List<FilePair>();
        var @new = new List<FilePair>();
        var notEquals = new List<(FilePair filePair, string? message, NotEqual notEqual)>();
        var delete = new List<string>
        {
            fakeReceivedTextFile
        };

        return ParseVerify(@new, notEquals, delete, equal);
    }

    static async Task ParseVerify(
        List<FilePair> @new,
        List<(FilePair filePair, string? message, NotEqual notEqual)> notEquals,
        List<string> delete,
        List<FilePair> equal,
        [CallerFilePath] string sourceFile = "")
    {
        var exceptionMessage = await VerifyExceptionMessageBuilder.Build(projectDirectory, @new, notEquals, delete, equal);

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