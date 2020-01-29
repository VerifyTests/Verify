using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiffEngine;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

public partial class Tests :
    VerifyBase
{
    static ResolvedDiffTool tool;

    static Tests()
    {
        var diffToolPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../../FakeDiffTool/bin/FakeDiffTool.exe"));
        tool = new ResolvedDiffTool(
            name: "FakeDiffTool",
            exePath: diffToolPath,
            buildArguments: (path1, path2) => $"\"{path1}\" \"{path2}\"",
            isMdi: false,
            supportsAutoRefresh: true);

        DiffTools.ResolvedDiffTools = new List<ResolvedDiffTool>
        {
            tool
        };

        DiffTools.ExtensionLookup = new Dictionary<string, ResolvedDiffTool>
        {
            {"txt", tool},
            {"knownBin", tool},
        };
        var binPath = EmptyFiles.Files["jpg"];
        EmptyFiles.Files = new Dictionary<string, EmptyFile>
        {
            {"knownBin", binPath},
        };

        SharedVerifySettings.RegisterFileConverter<TypeToSplit>(
            "txt",
            (split, settings) => new ConversionResult(
                split.Info,
                new List<Stream>
                {
                    new MemoryStream(Encoding.UTF8.GetBytes(split.Property1)),
                    new MemoryStream(Encoding.UTF8.GetBytes(split.Property2))
                }));
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
            var command = tool.BuildCommand(pair.Received,pair.Verified);
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

    public Tests(ITestOutputHelper output) :
        base(output)
    {
        ClipboardCapture.Clear();
    }
}