#if DEBUG
using DiffEngine;
using EmptyFiles;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public partial class Tests
{
    static string toolPath = Path.GetFullPath(Path.Combine(AttributeReader.GetSolutionDirectory(), "FakeDiffTool/bin/FakeDiffTool.exe"));

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
            targetLeftArguments: (tempFile, targetFile) => $"\"{targetFile}\" \"{tempFile}\"",
            targetRightArguments: (tempFile, targetFile) => $"\"{tempFile}\" \"{targetFile}\"",
            exePath: toolPath,
            binaryExtensions: new[] {"knownBin","AlwaysPassBin"});
        var binPath = AllFiles.Files["jpg"];
        var knownBinPath = Path.ChangeExtension(binPath.Path, "knownBin");
        File.Copy(binPath.Path, knownBinPath, true);
        AllFiles.UseFile(Category.Image, knownBinPath);
        var alwaysPassBinPath = Path.ChangeExtension(binPath.Path, "AlwaysPassBin");
        File.Copy(binPath.Path, alwaysPassBinPath, true);
        AllFiles.UseFile(Category.Image, alwaysPassBinPath);

        VerifierSettings.RegisterFileConverter<TypeToSplit>(
            (split, _) => new(
                split.Info,
                new List<Target>
                {
                    new("txt", split.Property1),
                    new("txt", split.Property2)
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

            var verifiedCommands = ProcessCleanup.Commands
                .Where(x => x.Command.Contains("verified"))
                .Select(x => x.Command);
            var commands = string.Join(Environment.NewLine, verifiedCommands);
            string message;
            if (isRunning)
            {
                message = "Expected command running";
            }
            else
            {
                message = "Expected command not running";
            }

            throw new($@"{message}
{command}
Commands:
{commands}");
        }
    }

    static string BuildCommand(FilePair pair)
    {
        return $"\"{toolPath}\" \"{pair.Received}\" \"{pair.Verified}\"";
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

    static Task<VerifyException> Throws(Func<Task> testCode)
    {
        return Assert.ThrowsAsync<VerifyException>(testCode);
    }

    public Tests()
    {
        PrefixUnique.Clear();
        ClipboardCapture.Clear();
    }
}
#endif