using System;
using System.Diagnostics;
using System.IO;
using EmptyFiles;

namespace DiffEngine
{
    /// <summary>
    /// Manages diff tools processes.
    /// </summary>
    public static class DiffRunner
    {
        static uint maxInstancesToLaunch = uint.MaxValue;
        static uint launchedInstances;

        public static void MaxInstancesToLaunch(uint value)
        {
            maxInstancesToLaunch = value;
        }

        /// <summary>
        /// Find and kill a diff tool process.
        /// </summary>
        public static void Kill(string tempFile, string targetFile)
        {
            Guard.AgainstNullOrEmpty(tempFile, nameof(tempFile));
            Guard.AgainstNullOrEmpty(targetFile, nameof(targetFile));
            var extension = Extensions.GetExtension(tempFile);
            if (!DiffTools.TryFind(extension, out var diffTool))
            {
                return;
            }

            var command = diffTool.BuildCommand(tempFile, targetFile);

            if (diffTool.IsMdi)
            {
                return;
            }

            ProcessCleanup.Kill(command);
        }

        /// <summary>
        /// Launch a diff tool for the given paths.
        /// </summary>
        public static void Launch(string tempFile, string targetFile)
        {
            Guard.AgainstNullOrEmpty(tempFile, nameof(tempFile));
            Guard.AgainstNullOrEmpty(targetFile, nameof(targetFile));
            var extension = Extensions.GetExtension(tempFile);
            if (launchedInstances >= maxInstancesToLaunch)
            {
                return;
            }

            if (!DiffTools.TryFind(extension, out var diffTool))
            {
                return;
            }

            //TODO: throw if both dont exist
            if (!File.Exists(tempFile))
            {
                if (!AllFiles.TryCreateFile(tempFile, true))
                {
                    return;
                }
            }

            if (!File.Exists(targetFile))
            {
                if (!AllFiles.TryCreateFile(targetFile, true))
                {
                    return;
                }
            }

            launchedInstances++;

            Launch(diffTool, tempFile, targetFile);
        }

        static void Launch(ResolvedDiffTool tool, string tempFile, string targetFile)
        {
            Guard.AgainstNull(tool, nameof(tool));
            var command = tool.BuildCommand(tempFile, targetFile);
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

            var arguments = tool.BuildArguments(tempFile, targetFile);
            try
            {
                Process.Start(tool.ExePath, arguments);
            }
            catch (Exception exception)
            {
                var message = $@"Failed to launch diff tool.
{tool.ExePath} {arguments}";
                throw new Exception(message, exception);
            }
        }
    }
}