using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

public class Tests :
    VerifyBase
{
    static ResolvedDiffTool tool;

    static Tests()
    {
        BuildServerDetector.Detected = false;
        var diffToolPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,"../../../../FakeDiffTool/bin/FakeDiffTool.exe"));
        tool = new ResolvedDiffTool(
            name: "FakeDiffTool",
            exePath: diffToolPath,
            shouldTerminate: false,
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
    [InlineData(false)]
    [InlineData( true)]
    public Task Text(bool hasExistingReceived)
    {
        return RunTest(
            "txt",
            () => "someText",
            () => "someOtherText",
            hasMatchingDiffTool: true,
            hasExistingReceived);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public Task Stream(
        bool hasMatchingDiffTool,
        bool hasExistingReceived)
    {
        var extension = hasMatchingDiffTool ? "knownBin" : "unknownBin";

        return RunTest(
            extension: extension,
            () => new MemoryStream(new byte[] {1}),
            () => new MemoryStream(new byte[] {2}),
            hasMatchingDiffTool,
            hasExistingReceived);
    }

    async Task RunTest(
        string extension,
        Func<object> initialTarget,
        Func<object> secondTarget,
        bool hasMatchingDiffTool,
        bool hasExistingReceived)
    {
        var settings = new VerifySettings();
        settings.UseExtension(extension);

        var danglingFile = Path.Combine(SourceDirectory, $"{Context.UniqueTestName}.01.verified.{extension}");
        var verified = Path.Combine(SourceDirectory, $"{Context.UniqueTestName}.verified.{extension}");
        var received = Path.Combine(SourceDirectory, $"{Context.UniqueTestName}.received.{extension}");

        var command = tool.BuildCommand(new FilePair(extension, received, verified));

        SetInitialState(danglingFile, verified, received,hasExistingReceived);

        await InitialVerify(initialTarget, hasMatchingDiffTool, settings, command, verified, received);

        RunClipboardCommand();
        AssertNotExists(danglingFile);

        await ReVerify(initialTarget, settings, command, received, verified);

        await InitialVerify(secondTarget, hasMatchingDiffTool, settings, command, verified, received);

        RunClipboardCommand();

        await ReVerify(secondTarget, settings, command, received, verified);
    }

    async Task ReVerify(Func<object> target, VerifySettings settings, string command, string received, string verified)
    {
        ProcessCleanup.RefreshCommands();
        await Verify(target(), settings);
        ProcessCleanup.RefreshCommands();
        AssertProcessNotRunning(command);

        AssertNotExists(received);
        AssertExists(verified);
    }

    async Task InitialVerify(Func<object> target, bool hasMatchingDiffTool, VerifySettings settings, string command, string verified, string received)
    {
        var exception = await Throws(() => Verify(target(), settings));
        ProcessCleanup.RefreshCommands();
        AssertProcess(command, hasMatchingDiffTool);
        if (hasMatchingDiffTool)
        {
            AssertExists(verified);
        }

        AssertExists(received);
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