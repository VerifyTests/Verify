using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
        BuildServerDetector.Detected = false;
        var diffToolPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../../../FakeDiffTool/bin/FakeDiffTool.exe"));
        tool = new ResolvedDiffTool(
            name: "FakeDiffTool",
            exePath: diffToolPath,
            buildArguments: pair => $"\"{pair.Received}\" \"{pair.Verified}\"",
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
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public Task Text(
        bool hasExistingReceived,
        bool autoVerify)
    {
        return RunTest(
            "txt",
            () => "someText",
            () => "someOtherText",
            hasMatchingDiffTool: true,
            hasExistingReceived,
            autoVerify);
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(false, false, true)]
    [InlineData(true, true, true)]
    public Task Stream(
        bool hasMatchingDiffTool,
        bool hasExistingReceived,
        bool autoVerify)
    {
        var extension = hasMatchingDiffTool ? "knownBin" : "unknownBin";

        return RunTest(
            extension: extension,
            () => new MemoryStream(new byte[] {1}),
            () => new MemoryStream(new byte[] {2}),
            hasMatchingDiffTool,
            hasExistingReceived,
            autoVerify);
    }

    async Task RunTest(
        string extension,
        Func<object> initialTarget,
        Func<object> secondTarget,
        bool hasMatchingDiffTool,
        bool hasExistingReceived,
        bool autoVerify)
    {
        var settings = new VerifySettings();
        settings.UseExtension(extension);
        if (autoVerify)
        {
            settings.AutoVerify();
        }

        var danglingFile = Path.Combine(SourceDirectory, $"{Context.UniqueTestName}.01.verified.{extension}");
        var verified = Path.Combine(SourceDirectory, $"{Context.UniqueTestName}.verified.{extension}");
        var received = Path.Combine(SourceDirectory, $"{Context.UniqueTestName}.received.{extension}");
        var pair = new FilePair(extension, received, verified);

        DeleteAll(danglingFile, verified, received);
        File.WriteAllText(danglingFile, "");

        if (hasExistingReceived)
        {
            File.WriteAllText(received, "");
        }

        await InitialVerify(initialTarget, hasMatchingDiffTool, settings, pair);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        AssertNotExists(danglingFile);

        await ReVerify(initialTarget, settings, pair);

        await InitialVerify(secondTarget, hasMatchingDiffTool, settings, pair);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        await ReVerify(secondTarget, settings, pair);
    }

    static void DeleteAll(params string[] files)
    {
        foreach (var file in files)
        {
            File.Delete(file);
        }
    }

    async Task ReVerify(Func<object> target, VerifySettings settings, FilePair pair)
    {
        var command = tool.BuildCommand(pair);
        ProcessCleanup.RefreshCommands();
        await Verify(target(), settings);
        ProcessCleanup.RefreshCommands();
        AssertProcessNotRunning(command);

        AssertNotExists(pair.Received);
        AssertExists(pair.Verified);
    }

    async Task InitialVerify(Func<object> target, bool hasMatchingDiffTool, VerifySettings settings, FilePair pair)
    {
        var command = tool.BuildCommand(pair);
        if (settings.autoVerify)
        {
            await Verify(target(), settings);
            AssertExists(pair.Verified);
        }
        else
        {
            var exception = await Throws(() => Verify(target(), settings));
            ProcessCleanup.RefreshCommands();
            AssertProcess(command, hasMatchingDiffTool);
            if (hasMatchingDiffTool)
            {
                AssertExists(pair.Verified);
            }

            AssertExists(pair.Received);
        }
    }

    static void SetInitialState(
        string danglingFile,
        string verified,
        string received,
        bool hasExistingReceived)
    {
        File.Delete(danglingFile);
        File.WriteAllText(danglingFile, "");

        File.Delete(verified);
        File.Delete(received);
        if (hasExistingReceived)
        {
            File.WriteAllText(received, "");
        }
    }

    static void AssertProcessNotRunning(string command)
    {
        Assert.False(ProcessCleanup.IsRunning(command));
    }

    static void AssertProcessRunning(string command)
    {
        Assert.True(ProcessCleanup.IsRunning(command));
    }

    static void AssertProcess(string command, bool isRunning)
    {
        Assert.Equal(isRunning, ProcessCleanup.IsRunning(command));
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