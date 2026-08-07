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
        var message = VerifyExceptionMessageBuilder.BuildInline(
            directory,
            Dir("MyTests.cs"),
            10,
            isNew: true,
            "the new content",
            null,
            Dir("obj/VerifyInline/abc.received.txt"),
            Dir("obj/VerifyInline/abc.expected.txt"),
            Dir("obj/VerifyInline/abc.inlinepatch"),
            []);
        return Verifier.Verify(message);
    }

    [Fact]
    public Task InlineNotEqualWithDelete()
    {
        var message = VerifyExceptionMessageBuilder.BuildInline(
            directory,
            Dir("MyTests.cs"),
            12,
            isNew: false,
            "received text",
            "expected text",
            Dir("obj/VerifyInline/def.received.txt"),
            Dir("obj/VerifyInline/def.expected.txt"),
            Dir("obj/VerifyInline/def.inlinepatch"),
            [Dir("MyTests.OldTest.verified.txt")]);
        return Verifier.Verify(message);
    }

    static Task BuildVerify(
        IReadOnlyCollection<NewResult> @new,
        IReadOnlyCollection<NotEqualResult> notEquals,
        IReadOnlyCollection<string> delete,
        IReadOnlyCollection<FilePair> equal)
    {
        var message = VerifyExceptionMessageBuilder.Build(
            directory,
            @new,
            notEquals,
            delete,
            equal);

        return Verifier.Verify(message);
    }
}
