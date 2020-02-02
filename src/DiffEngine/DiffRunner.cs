﻿using System.Diagnostics;
using System.IO;

namespace DiffEngine
{
    public static class DiffRunner
    {
        public static void TryKillProcessIfSupported(string extension, string path1, string path2)
        {
            Guard.AgainstNullOrEmpty(extension, nameof(extension));
            Guard.AgainstNullOrEmpty(path1, nameof(path1));
            Guard.AgainstNullOrEmpty(path2, nameof(path2));
            if (!DiffTools.TryFind(extension, out var diffTool))
            {
                return;
            }
            var command = diffTool.BuildCommand(path1, path2);

            if (diffTool.IsMdi)
            {
                return;
            }

            ProcessCleanup.Kill(command);
        }

        public static void TryLaunch(string extension, string path1, string path2)
        {
            Guard.AgainstNullOrEmpty(path1, nameof(path1));
            Guard.AgainstNullOrEmpty(path2, nameof(path2));
            if (!DiffTools.TryFind(extension, out var diffTool))
            {
                return;
            }
            //TODO: throw if both dont exist
            if (!File.Exists(path1))
            {
                if (!EmptyFilesWrapper.TryWriteEmptyFile(extension, path1))
                {
                    return;
                }
            }

            if (!File.Exists(path2))
            {
                if (!EmptyFilesWrapper.TryWriteEmptyFile(extension, path2))
                {
                    return;
                }
            }

            Launch(diffTool, path1, path2);
        }

        internal static void Launch(ResolvedDiffTool tool, string path1, string path2)
        {
            Guard.AgainstNull(tool, nameof(tool));
            var command = tool.BuildCommand(path1, path2);
            var isDiffToolRunning = ProcessCleanup.IsRunning(command);
            if (isDiffToolRunning)
            {
                if (tool.SupportsAutoRefresh)
                {
                    return;
                }

                if (!tool.IsMdi)
                {
                    ProcessCleanup.Kill(command);
                }
            }

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = tool.ExePath,
                    Arguments = tool.BuildArguments(path1, path2),
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };
            process.StartWithCatch();
        }
    }
}