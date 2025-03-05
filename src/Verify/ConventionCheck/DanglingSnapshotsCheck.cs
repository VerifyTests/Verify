#pragma warning disable CS1998

namespace VerifyTests;

[Experimental("DanglingSnapshotsCheck")]
public static class DanglingSnapshotsCheck
{
    static ConcurrentBag<string>? trackedVerifiedFiles;

    internal static void TrackVerifiedFile(string path) => trackedVerifiedFiles?.Add(path);

    public static void Run(string projectDirectory)
    {
        if (!BuildServerDetector.Detected)
        {
            return;
        }

        foreach (var file in Directory.EnumerateFiles(projectDirectory, "*.verified.*", SearchOption.AllDirectories))
        {
            if (!trackedVerifiedFiles!.Contains(file))
            {
                throw new VerifyCheckException($"The file {file} has not been tracked yet.");
            }
        }
    }

    [ModuleInitializer]
    internal static void Init()
    {
        if (BuildServerDetector.Detected)
        {
            trackedVerifiedFiles = [];
        }
    }
}