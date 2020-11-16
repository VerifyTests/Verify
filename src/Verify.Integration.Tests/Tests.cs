#if DEBUG
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiffEngine;
using EmptyFiles;
using VerifyTests;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

[UsesVerify]
public partial class Tests :
    XunitContextBase
{
    static string diffToolPath = Path.GetFullPath(Path.Combine(AssemblyLocation.CurrentDirectory, "../../../../FakeDiffTool/bin/FakeDiffTool.exe"));

    static Tests()
    {
        BuildServerDetector.Detected = false;
        DiffRunner.Disabled = false;
        DiffTools.AddTool(
            name: "MyTools",
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

        VerifierSettings.RegisterFileConverter<TypeToSplit>(
            (split, _) => new ConversionResult(
                split.Info,
                new List<ConversionStream>
                {
                    new("txt", new MemoryStream(FileHelpers.Utf8NoBOM.GetBytes(split.Property1))),
                    new("txt", new MemoryStream(FileHelpers.Utf8NoBOM.GetBytes(split.Property2)))
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

            throw InnerVerifier.exceptionBuilder($@"{message}
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

    public Tests(ITestOutputHelper output) :
        base(output)
    {
        ClipboardCapture.Clear();
    }
}
#endif