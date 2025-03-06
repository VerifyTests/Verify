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

            var suffix = file[directory.Length..];
            if (IfFileUnique(suffix))
            {
                continue;
            }

            untrackedFiles.Add(suffix);
        }

        if (untrackedFiles.Count == 0)
        {
            return;
        }

        var message = $"""
                       The following files have not been tracked:
                        * {string.Join("\n * ", untrackedFiles)}
                       """;
        if (BuildServerDetector.IsAzureDevops)
        {
            Environment.FailFast(message);
        }

        throw new(message);
    }

    static bool IfFileUnique(string file) =>
        file.Contains(".Net") ||
        file.Contains(".DotNet") ||
        file.Contains(".Mono.") ||
        file.Contains(".OSX.") ||
        file.Contains(".Windows.") ||
        file.Contains(".Linux.");

    [ModuleInitializer]
    internal static void Init()
    {
        if (BuildServerDetector.Detected)
        {
            trackedVerifiedFiles = [];
        }
    }
}