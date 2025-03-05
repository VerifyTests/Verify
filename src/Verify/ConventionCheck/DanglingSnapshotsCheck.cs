#pragma warning disable VerifierSettingsTestAssembly
#pragma warning disable CS1998

namespace VerifyTests;

public static class DanglingSnapshotsCheck
{
    static ConcurrentBag<string>? trackedVerifiedFiles;

    internal static void TrackVerifiedFile(string path) => trackedVerifiedFiles?.Add(path);

    public static void Run()
    {
        if (!BuildServerDetector.Detected)
        {
            return;
        }

        var directory = AttributeReader.GetProjectDirectory(VerifierSettings.Assembly);
        foreach (var file in Directory.EnumerateFiles(directory, "*.verified.*", SearchOption.AllDirectories))
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