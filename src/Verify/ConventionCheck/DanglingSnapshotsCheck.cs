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
        List<string> untrackedFiles = [];
        foreach (var file in Directory.EnumerateFiles(directory, "*.verified.*", SearchOption.AllDirectories))
        {
            if (trackedVerifiedFiles!.Contains(file))
            {
                continue;
            }

            var suffix = file.Replace(directory, string.Empty);
            if (suffix.Contains(".Net") ||
                suffix.Contains(".DotNet") ||
                suffix.Contains(".Mono") ||
                suffix.Contains(".OSX.") ||
                suffix.Contains(".Windows.") ||
                suffix.Contains(".Linux."))
            {
                continue;
            }
            untrackedFiles.Add(suffix);
        }

        if (untrackedFiles.Count != 0)
        {
            throw new VerifyCheckException(
                $"""
                 The following files have not been tracked:
                  * {string.Join("\n * ", untrackedFiles)}
                 """);
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