using System;
using Verify;

/// <summary>
/// Not for public use.
/// </summary>
public partial class InnerVerifier
{
    Type testType;
    string directory;
    string testName;
    internal static Func<string, Exception> exceptionBuilder = null!;
    static Action<string, string> assert = null!;

    public static void Init(
        Func<string, Exception> exceptionBuilder,
        Func<Guid, int> guidIntOrNext,
        Func<DateTime, int> dateTimeIntOrNext,
        Func<DateTimeOffset, int> dateTimeOffsetIntOrNext,
        Action<string, string> assert)
    {
        InnerVerifier.assert = assert;
        InnerVerifier.exceptionBuilder = exceptionBuilder;
        Scrubber<Guid>.SetIntOrNext(guidIntOrNext);
        Scrubber<DateTime>.SetIntOrNext(dateTimeIntOrNext);
        Scrubber<DateTimeOffset>.SetIntOrNext(dateTimeOffsetIntOrNext);
    }

    public InnerVerifier(Type testType, string directory, string testName)
    {
        this.testType = testType;
        this.directory = directory;
        this.testName = testName;
    }

    FilePair GetFileNames(string extension, Namer namer, string? suffix = null)
    {
        return FileNameBuilder.GetFileNames(extension, suffix, namer, testType, directory, testName);
    }
}