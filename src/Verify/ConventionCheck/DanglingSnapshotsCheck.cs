﻿#pragma warning disable VerifierSettingsTestAssembly
#pragma warning disable CS1998

namespace VerifyTests;

static class DanglingSnapshotsCheck
{
    public enum OnFailure
    {
        Throw,
        FailFast
    }

    static ConcurrentBag<string>? trackedVerifiedFiles;

    internal static void TrackVerifiedFile(string path) => trackedVerifiedFiles?.Add(path);

    public static void Run(OnFailure onFailure)
    {
        if (!BuildServerDetector.Detected)
        {
            return;
        }

        var directory = AttributeReader.GetProjectDirectory(VerifierSettings.Assembly);
        var files = Directory.EnumerateFiles(directory, "*.verified.*", SearchOption.AllDirectories);
        CheckFiles(files, onFailure, directory);
    }

    internal static void CheckFiles(IEnumerable<string> filesOnDisk, OnFailure onFailure, string directory)
    {
        List<string> untracked = [];
        List<string> incorrectCase = [];
        foreach (var file in filesOnDisk)
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

            if (trackedVerifiedFiles!.Contains(file, StringComparer.OrdinalIgnoreCase))
            {
                incorrectCase.Add(suffix);
            }
            else
            {
                untracked.Add(suffix);
            }
        }

        if (untracked.Count == 0 && incorrectCase.Count == 0)
        {
            return;
        }

        var builder = new StringBuilder("Verify has detected the following issues with snapshot files:");
        if (untracked.Count > 0)
        {
            builder.AppendLine();
            builder.AppendLine("The following files have not been tracked:");
            foreach (var file in untracked)
            {
                builder.AppendLine($" * {file}");
            }
        }

        if(incorrectCase.Count> 0)
        {
            builder.AppendLine();
            builder.AppendLine("The following files have been tracked with incorrect case:");
            foreach (var file in incorrectCase)
            {
                builder.AppendLine($" * {file}");
            }
        }
        var message = builder.ToString();

        if (onFailure == OnFailure.FailFast)
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