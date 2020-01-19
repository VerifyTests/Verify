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
            {"jpg", tool},
        };
    }

    [Fact]
    public Task Text()
    {
        return RunTest(nameof(Text), "txt", () => "someText");
    }

    [Fact]
    public Task Stream()
    {
        var settings = new VerifySettings();
        settings.UseExtension("jpg");
        return RunTest(
            testName: nameof(Stream),
            extension: "jpg",
            () => new MemoryStream(new byte[] {1}));
    }

    [Fact]
    public Task StreamNoMatchingDiff()
    {
        return RunTest(
            testName: nameof(StreamNoMatchingDiff),
            extension: "png",
            () => new MemoryStream(new byte[] {1}),
            hasMatchingDiffTool: false);
    }

    async Task RunTest(string testName, string extension, Func<object> target, bool hasMatchingDiffTool = true)
    {
        var settings = new VerifySettings();
        settings.UseExtension(extension);

        Task Func() => Verify(target(), settings);

        var danglingFile = Path.Combine(SourceDirectory, $"Tests.{testName}.01.verified.{extension}");
        File.Delete(danglingFile);
        File.WriteAllText(danglingFile, "");

        var verified = Path.Combine(SourceDirectory, $"Tests.{testName}.verified.{extension}");
        File.Delete(verified);

        var received = Path.Combine(SourceDirectory, $"Tests.{testName}.received.{extension}");
        File.Delete(received);

        var exception = await Throws(Func);
        var command = tool.BuildCommand(new FilePair(extension, received, verified));
        ProcessCleanup.RefreshCommands();
        AssertProcess(command, hasMatchingDiffTool);
        if (hasMatchingDiffTool)
        {
            AssertExists(verified);
        }

        AssertExists(received);
        RunClipboardCommand();
        AssertNotExists(danglingFile);

        ProcessCleanup.RefreshCommands();
        await Func();
        ProcessCleanup.RefreshCommands();
        AssertProcessNotRunning(command);

        AssertNotExists(received);
        AssertExists(verified);
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