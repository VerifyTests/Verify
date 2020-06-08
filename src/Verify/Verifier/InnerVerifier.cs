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

    public static void Init(Func<string, Exception> exceptionBuilder)
    {
        InnerVerifier.exceptionBuilder = exceptionBuilder;
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