using System;

partial class Verifier
{
    Type testType;
    string directory;
    string testName;
    static Func<string, Exception> exceptionBuilder= null!;
    static Action<string, string> assert= null!;

    public static void Init(
        Func<string, Exception> exceptionBuilder,
        Func<Guid, int> guidIntOrNext,
        Func<DateTime, int> dateTimeIntOrNext,
        Func<DateTimeOffset, int> dateTimeOffsetIntOrNext,
        Action<string, string> assert)
    {
        Verifier.assert = assert;
        Verifier.exceptionBuilder = exceptionBuilder;
        Scrubber<Guid>.SetIntOrNext(guidIntOrNext);
        Scrubber<DateTime>.SetIntOrNext(dateTimeIntOrNext);
        Scrubber<DateTimeOffset>.SetIntOrNext(dateTimeOffsetIntOrNext);
    }

    public Verifier(Type testType, string directory, string testName)
    {
        this.testType = testType;
        this.directory = directory;
        this.testName = testName;
    }
}