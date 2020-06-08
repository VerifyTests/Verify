#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiffEngine;
using EmptyFiles;
using Verify;
using Xunit;
using Xunit.Sdk;

public partial class Tests
{
    static string diffToolPath = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "../../../../FakeDiffTool/bin/FakeDiffTool.exe"));

    static Tests()
    {
        BuildServerDetector.Detected = false;
        DiffTools.AddTool(
            name:"MyTools",
            autoRefresh: true,
            isMdi: false,
            supportsText: true,
            requiresTarget: true,
            arguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
            exePath: diffToolPath,
            binaryExtensions: new[] {"knownBin"});
        var binPath = AllFiles.Files["jpg"];
        var newPath = Path.ChangeExtension(binPath.Path, "knownBin");
        File.Copy(binPath.Path, newPath, true);
        AllFiles.UseFile(Category.Image, newPath);

        SharedVerifySettings.RegisterFileConverter<TypeToSplit>(
            "txt",
            (split, settings) => new ConversionResult(
                split.Info,
                new List<Stream>
                {
                    new MemoryStream(FileHelpers.Utf8NoBOM.GetBytes(split.Property1)),
                    new MemoryStream(FileHelpers.Utf8NoBOM.GetBytes(split.Property2))
                }));
        DiffRunner.MaxInstancesToLaunch(int.MaxValue);
    }

    static void DeleteAll(params string[] files)
    {
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }

    static void AssertProcessNotRunning(string command)
    {
        Assert.False(ProcessCleanup.IsRunning(command));
    }

    static void AssertProcess(bool isRunning, params FilePair[] pairs)
    {
        foreach (var pair in pairs)
        {
            var command = BuildCommand(pair);
            if (isRunning == ProcessCleanup.IsRunning(command))
            {
                continue;
            }

            var commands = string.Join(Environment.NewLine, ProcessCleanup.Commands.Select(x => x.Command));
            string message;
            if (isRunning)
            {
                message = "Expected command running";
            }
            else
            {
                message = "Expected command not running";
            }

            throw new Exception($@"{message}
{command}
Commands:
{commands}");
        }
    }

    static string BuildCommand(FilePair pair)
    {
        return $"\"{diffToolPath}\" \"{pair.Received}\" \"{pair.Verified}\"";
    }

    static void RunClipboardCommand()
    {
        foreach (var line in ClipboardCapture
            .Read()
            .Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
        {
            var command = $"/c {line}";
            using var process = Process.Start("cmd.exe", command);
            process.WaitForExit();
        }
    }

    static void AssertExists(string file)
    {
        Assert.True(File.Exists(file));
    }

    static void AssertNotExists(string file)
    {
        Assert.False(File.Exists(file));
    }

    static Task<XunitException> Throws(Func<Task> testCode)
    {
        return Assert.ThrowsAsync<XunitException>(testCode);
    }

    public Tests()
    {
        ClipboardCapture.Clear();
    }
}
#endif