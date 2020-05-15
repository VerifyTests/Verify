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

    public static void Init(
        Func<string, Exception> exceptionBuilder,
        Func<Guid, int> guidIntOrNext,
        Func<DateTime, int> dateTimeIntOrNext,
        Func<DateTimeOffset, int> dateTimeOffsetIntOrNext)
    {
        InnerVerifier.exceptionBuilder = exceptionBuilder;
        SharedScrubber.SetIntOrNext(guidIntOrNext, dateTimeIntOrNext, dateTimeOffsetIntOrNext);
    }

    public InnerVerifier(Type testType, string directory, string testName)
    {
        this.testType = testType;
        this.directory = SharedVerifySettings.DeriveDirectory(testType, directory);
        this.testName = testName;
    }

    FilePair GetFileNames(string extension, Namer namer)
    {
        return FileNameBuilder.GetFileNames(extension, namer, testType, directory, testName);
    }

    FilePair GetFileNames(string extension, Namer namer, string suffix)
    {
        return FileNameBuilder.GetFileNames(extension, suffix, namer, testType, directory, testName);
    }
}