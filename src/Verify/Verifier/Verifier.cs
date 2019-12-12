using System;
using Verify;

partial class Verifier
{
    static Func<string, Exception> exceptionBuilder= null!;
    static Action<string, string> assert= null!;
    static Func<TestContext> getTestContext = null!;

    public static void Init(
        Func<string, Exception> exceptionBuilder,
        Func<Guid, int> guidIntOrNext,
        Func<DateTime, int> dateTimeIntOrNext,
        Func<DateTimeOffset, int> dateTimeOffsetIntOrNext,
        Action<string, string> assert,
        Func<TestContext> getTestContext)
    {
        Verifier.getTestContext= getTestContext;
        Verifier.assert = assert;
        Verifier.exceptionBuilder = exceptionBuilder;
        Scrubber<Guid>.SetIntOrNext(guidIntOrNext);
        Scrubber<DateTime>.SetIntOrNext(dateTimeIntOrNext);
        Scrubber<DateTimeOffset>.SetIntOrNext(dateTimeOffsetIntOrNext);
    }
}

class TestContext
{
    public Type TestType { get; }
    public string Directory { get; }
    public string TestName { get; }

    public TestContext(Type testType, string directory, string testName)
    {
        TestType = testType;
        Directory = directory;
        TestName = testName;
    }
}