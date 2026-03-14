using FilePair = VerifyTests.FilePair;

public class VerifyExceptionMessageBuilderTests
{
    static string projectDirectory = AttributeReader.GetProjectDirectory();
    static string fakeFilePrefix = CurrentFile.Relative("VerifyExceptionMessageBuilderTests.Fake");
    static string receivedBin = $"{fakeFilePrefix}.received.bin";
    static string verifiedBin = $"{fakeFilePrefix}.verified.bin";
    static string receivedTxt = $"{fakeFilePrefix}.received.txt";
    static string verifiedTxt = $"{fakeFilePrefix}.verified.txt";
    static string fakeReceivedTextFile = CurrentFile.Relative("VerifyExceptionMessageBuilderTests.Fake.received.txt");

    [Fact]
    public Task Empty() =>
        BuildVerify([], [], [], []);

    [Fact]
    public Task SingleNew_Text() =>
        BuildVerify(
            @new:
            [
                new(new("txt", receivedTxt, verifiedTxt), new("the text content"))
            ],
            notEquals: [],
            delete: [],
            equal: []);

    [Fact]
    public Task SingleNew_Binary() =>
        BuildVerify(
            @new:
            [
                new(new("bin", receivedBin, verifiedBin), null)
            ],
            notEquals: [],
            delete: [],
            equal: []);

    [Fact]
    public Task SingleNotEqual_Text() =>
        BuildVerify(
            @new: [],
            notEquals:
            [
                new(new("txt", receivedTxt, verifiedTxt), null, new("received content"), new("verified content"))
            ],
            delete: [],
            equal: []);

    [Fact]
    public Task SingleNotEqual_WithMessage() =>
        BuildVerify(
            @new: [],
            notEquals:
            [
                new(new("txt", receivedTxt, verifiedTxt), "Comparer reported difference", new("received content"), new("verified content"))
            ],
            delete: [],
            equal: []);

    [Fact]
    public Task SingleNotEqual_Binary() =>
        BuildVerify(
            @new: [],
            notEquals:
            [
                new(new("bin", receivedBin, verifiedBin), null, null, null)
            ],
            delete: [],
            equal: []);

    [Fact]
    public Task SingleNotEqual_BinaryWithMessage() =>
        BuildVerify(
            @new: [],
            notEquals:
            [
                new(new("bin", receivedBin, verifiedBin), "Binary files differ", null, null)
            ],
            delete: [],
            equal: []);

    [Fact]
    public Task SingleDelete() =>
        BuildVerify(
            @new: [],
            notEquals: [],
            delete: [fakeReceivedTextFile],
            equal: []);

    [Fact]
    public Task SingleEqual() =>
        BuildVerify(
            @new: [],
            notEquals: [],
            delete: [],
            equal:
            [
                new("txt", receivedTxt, verifiedTxt)
            ]);

    [Fact]
    public Task AllCategories()
    {
        var @new = new List<NewResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), new("new content"))
        };
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), null, new("received"), new("verified"))
        };
        var delete = new List<string>
        {
            fakeReceivedTextFile
        };
        var equal = new List<FilePair>
        {
            new("txt", receivedTxt, verifiedTxt)
        };

        return BuildVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task MultipleNew_MixedTextAndBinary()
    {
        var @new = new List<NewResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), new("text content")),
            new(new("bin", receivedBin, verifiedBin), null)
        };

        return BuildVerify(@new, [], [], []);
    }

    [Fact]
    public Task MultipleNotEqual_MixedMessageAndNoMessage()
    {
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", receivedTxt, verifiedTxt), null, new("received text"), new("verified text")),
            new(new("txt", receivedTxt, verifiedTxt), "Custom comparison message", new("received2"), new("verified2"))
        };

        return BuildVerify([], notEquals, [], []);
    }

    static Task BuildVerify(
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

        return Verifier.Verify(message);
    }
}
