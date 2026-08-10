using FilePair = VerifyTests.FilePair;

public class ExceptionMessageFormatSamples
{
    static string directory = AttributeReader.GetProjectDirectory();
    static string Dir(string name) => Path.Combine(directory, name);

    [Fact]
    public Task AllCategories()
    {
        var @new = new List<NewResult>
        {
            new(new("txt", Dir("MyTests.Test1.received.txt"), Dir("MyTests.Test1.verified.txt")), new("the new content"))
        };
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", Dir("MyTests.Test2.received.txt"), Dir("MyTests.Test2.verified.txt")), null, new("received text"), "verified text")
        };
        var delete = new List<string>
        {
            Dir("MyTests.OldTest.verified.txt")
        };
        var equal = new List<FilePair>
        {
            new("txt", Dir("MyTests.Test3.received.txt"), Dir("MyTests.Test3.verified.txt"))
        };

        return BuildVerify(@new, notEquals, delete, equal);
    }

    [Fact]
    public Task NotEqualWithMessage()
    {
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", Dir("MyTests.Test1.received.txt"), Dir("MyTests.Test1.verified.txt")), "The comparer reported a difference", new("received text"), "verified text")
        };

        return BuildVerify([], notEquals, [], []);
    }

    [Fact]
    public Task InlineNew()
    {
        var inline = new InlineSection(
            Dir("MyTests.cs"),
            10,
            IsNew: true,
            "the new content",
            null,
            Staged("abc"));
        return BuildVerify([], [], [], [], inline);
    }

    [Fact]
    public Task InlineNotEqualWithDelete()
    {
        var inline = new InlineSection(
            Dir("MyTests.cs"),
            12,
            IsNew: false,
            "received text",
            "expected text",
            Staged("def"));
        return BuildVerify([], [], [Dir("MyTests.OldTest.verified.txt")], [], inline);
    }

    /// <summary>
    /// Only the first target is inlined, so a verification can fail on the literal and on a file
    /// at the same time and both have to survive into one message.
    /// </summary>
    [Fact]
    public Task InlineAndFileTogether()
    {
        var notEquals = new List<NotEqualResult>
        {
            new(new("txt", Dir("MyTests.Test1#01.received.txt"), Dir("MyTests.Test1#01.verified.txt")), null, new("received text"), "verified text")
        };
        var inline = new InlineSection(
            Dir("MyTests.cs"),
            14,
            IsNew: false,
            "inline received",
            "inline expected",
            null);
        return BuildVerify([], notEquals, [Dir("MyTests.OldTest.verified.txt")], [], inline);
    }

    static StagedInline Staged(string name) =>
        new(
            Dir($"obj/VerifyInline/{name}.received.txt"),
            Dir($"obj/VerifyInline/{name}.expected.txt"),
            Dir($"obj/VerifyInline/{name}.inlinepatch"));

    static Task BuildVerify(
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        IReadOnlyCollection<string> delete,
        IReadOnlyCollection<FilePair> equal,
        InlineSection? inline = null)
    {
        var message = VerifyExceptionMessageBuilder.Build(
            directory,
            @new,
            notEquals,
            delete,
            equal,
            inline);

        return Verifier.Verify(message);
    }
}
