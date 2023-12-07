using VerifyTests.ExceptionParsing;
using FilePair = VerifyTests.FilePair;

[UsesVerify]
public class ExceptionParsingTests
{
    static string projectDirectory = AttributeReader.GetProjectDirectory();
    static string fakeFilePrefix = CurrentFile.Relative("ExceptionParsingTests.Fake");
    static string receivedBin = $"{fakeFilePrefix}.received.bin";
    static string verifiedBin = $"{fakeFilePrefix}.verified.bin";
    static string receivedTxt = $"{fakeFilePrefix}.received.txt";
    static string verifiedTxt = $"{fakeFilePrefix}.verified.txt";
    static string fakeReceivedTextFile = CurrentFile.Relative("ExceptionParsingTests.Fake.received.txt");
    static string fakeReceivedBinFile = CurrentFile.Relative("ExceptionParsingTests.Fake.received.bin");

    [Fact]
    public Task Error_EmptyList() =>
        Throws(() => Parser.Parse([Environment.NewLine]))
            .IgnoreStackTrace();

    [Fact]
    public Task Error_EmptyDirectory() =>
        Throws(() => Parser.Parse(["Directory: "]))
            .IgnoreStackTrace();

    [Fact]
    public Task Empty() =>
        ParseVerify(
            Array.Empty<NewResult>(),
            Array.Empty<NotEqualResult>(),
            Array.Empty<string>(),
            Array.Empty<FilePair>());

    [Fact]
    public Task WithMessage()
    {
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), "TheMessage", new("receivedText"), new("verifiedText")),
            new(new("bin", receivedBin, verifiedBin), "TheMessage", null, null)
        };

        return ParseVerify(
            Array.Empty<NewResult>(),
            notEquals,
            Array.Empty<string>(),
            Array.Empty<FilePair>());
    }

    [Fact]
    public Task SingleEqual()
    {
        var equal = new List<FilePair>
        {
            new("txt", receivedTxt, verifiedTxt)
        };

        return ParseVerify(
            Array.Empty<NewResult>(),
            Array.Empty<NotEqualResult>(),
            Array.Empty<string>(),
            equal);
    }

    [Fact]
    public Task SingleNew()
    {
        var @new = new List<NewResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), new("contents"))
        };

        return ParseVerify(
            @new,
            Array.Empty<NotEqualResult>(),
            Array.Empty<string>(),
            Array.Empty<FilePair>());
    }

    [Fact]
    public Task Nunit()
    {
        var exceptionMessage = $"""
                                VerifyException : Directory: {Environment.CurrentDirectory}
                                NotEqual:
                                  - Received: XAMLCombinerTests.TestOutput.received.xaml
                                    Verified: XAMLCombinerTests.TestOutput.verified.xaml

                                FileContent:

                                NotEqual:

                                Received: XAMLCombinerTests.TestOutput.received.xaml
                                <?xml version="1.0" encoding="utf-8"?>
                                <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:sys="clr-namespace:System;assembly=mscorlib" xmlns:sys_0="clr-namespace:System;assembly=System.Runtime">
                                  <Style x:Key="Control1" />
                                  <Color x:Key="Control1_Color">#FF2B579A</Color>
                                  <sys:String x:Key="string1">stringValue</sys:String>
                                  <Style x:Key="Control2" />
                                  <Color x:Key="Control2_Color">#FF2B579A</Color>
                                  <sys:String x:Key="string2">stringValue</sys:String>
                                  <sys_0:String x:Key="string3">stringValue</sys_0:String>
                                  <Style TargetType="Block" />
                                </ResourceDictionary>
                                Verified: XAMLCombinerTests.TestOutput.verified.xaml
                                """;

        var result = Parser.Parse(exceptionMessage);
        return Verify(result);
    }

    [Fact]
    public Task MsTest()
    {
        var exceptionMessage = $"""
                                Test method TheTests.XAMLCombinerTests.TestOutput threw exception:
                                VerifyException: Directory: {Environment.CurrentDirectory}
                                NotEqual:
                                  - Received: XAMLCombinerTests.TestOutput.received.xaml
                                    Verified: XAMLCombinerTests.TestOutput.verified.xaml

                                FileContent:

                                NotEqual:

                                Received: XAMLCombinerTests.TestOutput.received.xaml
                                <?xml version="1.0" encoding="utf-8"?>
                                <ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" xmlns:sys="clr-namespace:System;assembly=mscorlib" xmlns:sys_0="clr-namespace:System;assembly=System.Runtime">
                                  <Style x:Key="Control1" />
                                  <Color x:Key="Control1_Color">#FF2B579A</Color>
                                  <sys:String x:Key="string1">stringValue</sys:String>
                                  <Style x:Key="Control2" />
                                  <Color x:Key="Control2_Color">#FF2B579A</Color>
                                  <sys:String x:Key="string2">stringValue</sys:String>
                                  <sys_0:String x:Key="string3">stringValue</sys_0:String>
                                  <Style TargetType="Block" />
                                </ResourceDictionary>
                                Verified: XAMLCombinerTests.TestOutput.verified.xaml
                                """;

        var result = Parser.Parse(exceptionMessage);
        return Verify(result);
    }

    [Fact]
    public Task SingleNotEqual()
    {
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), null, new("receivedText"), new("verifiedText"))
        };

        return ParseVerify(
            Array.Empty<NewResult>(),
            notEquals,
            Array.Empty<string>(),
            Array.Empty<FilePair>());
    }

    [Fact]
    public Task MultipleItem()
    {
        var equal = new List<FilePair>
        {
            new("txt", receivedTxt, verifiedTxt),
            new("bin", receivedBin, verifiedBin)
        };
        var @new = new List<NewResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), new("the content")),
            new(new("bin", receivedBin, verifiedBin), null)
        };
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), null, new("receivedText"), new("verifiedText")),
            new(new("bin", receivedBin, verifiedBin), null, null, null)
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
            new("txt", receivedTxt, verifiedTxt)
        };
        var @new = new List<NewResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), new("the content"))
        };
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), null, new("receivedText"), new("verifiedText"))
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
        var delete = new List<string>
        {
            fakeReceivedTextFile
        };
        return ParseVerify(
            Array.Empty<NewResult>(),
            Array.Empty<NotEqualResult>(),
            delete,
            Array.Empty<FilePair>());
    }

    static Task ParseVerify(
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        IReadOnlyCollection<string> delete,
        IReadOnlyCollection<FilePair> equal)
    {
        var message = VerifyExceptionMessageBuilder.Build(
            projectDirectory,
            @new,
            notEquals,
            delete,
            equal);

        var result = Parser.Parse(message);
        return Verify(
            new
            {
                message,
                result
            });
    }
}