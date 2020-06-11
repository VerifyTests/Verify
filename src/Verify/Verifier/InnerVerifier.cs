using System;
using Verify;

/// <summary>
/// Not for public use.
/// </summary>
public partial class InnerVerifier
{
    string directory;
    string testName;
    internal static Func<string, Exception> exceptionBuilder = null!;

    public static void Init(Func<string, Exception> exceptionBuilder)
    {
        InnerVerifier.exceptionBuilder = exceptionBuilder;
    }

    public InnerVerifier(string testName, string sourceFile)
    {
        directory = SharedVerifySettings.DeriveDirectory(sourceFile);
        this.testName = testName;
    }

    FilePair GetFileNames(string extension, Namer namer)
    {
        return FileNameBuilder.GetFileNames(extension, namer, directory, testName);
    }

    FilePair GetFileNames(string extension, Namer namer, string suffix)
    {
        return FileNameBuilder.GetFileNames(extension, suffix, namer, directory, testName);
    }
}