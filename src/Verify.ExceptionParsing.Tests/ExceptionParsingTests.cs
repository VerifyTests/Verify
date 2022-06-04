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
    public Task Error_EmptyList() =>
        Throws(() => Parser.Parse(new[]
            {
                Environment.NewLine
            }))
            .IgnoreStackTrace();

    [Fact]
    public Task Error_EmptyDirectory() =>
        Throws(() => Parser.Parse(new[]
            {
                "Directory: "
            }))
            .IgnoreStackTrace();

    [Fact]
    public Task Empty()
    {
        var equal = new List<FilePair>();
        var @new = new List<NewResult>();
        var notEquals = new List<NotEqualResult>();
        var delete = new List<string>();

        return ParseVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task WithMessage()
    {
        var equal = new List<FilePair>();
        var @new = new List<NewResult>();
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", fakeFilePrefix), "TheMessage", "receivedText", "verifiedText"),
            new(new("bin", fakeFilePrefix), "TheMessage", null, null)
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
        var @new = new List<NewResult>();
        var notEquals = new List<NotEqualResult>();
        var delete = new List<string>();

        return ParseVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task SingleNew()
    {
        var equal = new List<FilePair>();
        var @new = new List<NewResult>
        {
            new(new("txt", fakeFilePrefix), "contents")
        };
        var notEquals = new List<NotEqualResult>();
        var delete = new List<string>();

        return ParseVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task Nunit()
    {
        var exceptionMessage = @$"VerifyException : Directory: {Environment.CurrentDirectory}
NotEqual:
  - Received: XAMLCombinerTests.TestOutput.received.xaml
    Verified: XAMLCombinerTests.TestOutput.verified.xaml

FileContent:

NotEqual:

Received: XAMLCombinerTests.TestOutput.received.xaml
<?xml version=""1.0"" encoding=""utf-8""?>
<ResourceDictionary xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:sys=""clr-namespace:System;assembly=mscorlib"" xmlns:sys_0=""clr-namespace:System;assembly=System.Runtime"">
  <Style x:Key=""Control1"" />
  <Color x:Key=""Control1_Color"">#FF2B579A</Color>
  <sys:String x:Key=""string1"">stringValue</sys:String>
  <Style x:Key=""Control2"" />
  <Color x:Key=""Control2_Color"">#FF2B579A</Color>
  <sys:String x:Key=""string2"">stringValue</sys:String>
  <sys_0:String x:Key=""string3"">stringValue</sys_0:String>
  <Style TargetType=""Block"" />
</ResourceDictionary>
Verified: XAMLCombinerTests.TestOutput.verified.xaml


";

        var result = Parser.Parse(exceptionMessage);
        return Verify(result);
    }

    [Fact]
    public Task SingleNotEqual()
    {
        var equal = new List<FilePair>();
        var @new = new List<NewResult>();
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", fakeFilePrefix), null, "receivedText", "verifiedText")
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
        var @new = new List<NewResult>
        {
            new(new("txt", fakeFilePrefix), "the content"),
            new(new("bin", fakeFilePrefix), null)
        };
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", fakeFilePrefix), null, "receivedText", "verifiedText"),
            new(new("bin", fakeFilePrefix), null, null, null)
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
        var @new = new List<NewResult>
        {
            new(new("txt", fakeFilePrefix), "the content")
        };
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", fakeFilePrefix), null, "receivedText", "verifiedText")
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
        var @new = new List<NewResult>();
        var notEquals = new List<NotEqualResult>();
        var delete = new List<string>
        {
            fakeReceivedTextFile
        };

        return ParseVerify(@new, notEquals, delete, equal);
    }

    static Task ParseVerify(
        List<NewResult> @new,
        List<NotEqualResult> notEquals,
        List<string> delete,
        List<FilePair> equal,
        [CallerFilePath] string sourceFile = "")
    {
        var exceptionMessage = VerifyExceptionMessageBuilder.Build(projectDirectory, @new, notEquals, delete, equal);

        var result = Parser.Parse(exceptionMessage);
        return Verify(
            new
            {
                exceptionMessage,
                result
            },
            sourceFile: sourceFile);
    }
}