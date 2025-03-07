#pragma warning disable VerifierSettingsTestAssembly
#pragma warning disable CS1998

namespace VerifyTests;

static class DanglingSnapshotsCheck
{
    static ConcurrentBag<string>? trackedVerifiedFiles;

    internal static void TrackVerifiedFile(string path) => trackedVerifiedFiles?.Add(path);

    public static void Run(bool failFast)
    {
        if (!BuildServerDetector.Detected)
        {
            return;
        }

        var directory = AttributeReader.GetProjectDirectory(VerifierSettings.Assembly);
        var files = Directory.EnumerateFiles(directory, "*.verified.*", SearchOption.AllDirectories);
        CheckFiles(files, trackedVerifiedFiles!, failFast, directory);
    }

    internal static void CheckFiles(IEnumerable<string> filesOnDisk, ConcurrentBag<string> trackedFiles, bool failFast, string directory)
    {
        static void AppendItems(StringBuilder builder, List<string> list, string title)
        {
            if (list.Count <= 0)
            {
                return;
            }

            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine(title);
            builder.AppendLine();
            foreach (var file in list)
            {
                builder.AppendLine($" * {file}");
            }
        }

        List<string> untracked = [];
        List<string> incorrectCase = [];
        foreach (var file in filesOnDisk)
        {
            if (trackedFiles.Contains(file))
            {
                continue;
            }

            var suffix = file[directory.Length..];
            suffix = suffix.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (IfFileUnique(suffix))
            {
                continue;
            }

            if (trackedFiles.Contains(file, StringComparer.OrdinalIgnoreCase))
            {
                incorrectCase.Add(suffix);
            }
            else
            {
                untracked.Add(suffix);
            }
        }

        if (untracked.Count == 0 &&
            incorrectCase.Count == 0)
        {
            return;
        }

        var builder = new StringBuilder(
            """

            Verify has detected the following issues with snapshot files:
            """);

        AppendItems(builder, untracked, "The following files have not been tracked:");
        AppendItems(builder, incorrectCase, "The following files have been tracked with incorrect case:");
        var message = builder.ToString();

        if (failFast)
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