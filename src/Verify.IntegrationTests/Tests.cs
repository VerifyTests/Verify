using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
            {"txt", tool}
        };
    }

    [Fact]
    public async Task Text()
    {
        ClipboardCapture.Clear();
        var danglingFile = Path.Combine(SourceDirectory, "Tests.Text.01.verified.txt");
        File.Delete(danglingFile);
        File.WriteAllText(danglingFile, "");

        var verified = Path.Combine(SourceDirectory, "Tests.Text.verified.txt");
        File.Delete(verified);

        var received = Path.Combine(SourceDirectory, "Tests.Text.received.txt");
        File.Delete(received);

        var exception = await Throws(() => Verify("someText"));
        var command = tool.BuildCommand(new FilePair("txt", received, verified));
        ProcessCleanup.RefreshCommands();
        Assert.True(ProcessCleanup.IsRunning(command));
        AssertExists(verified);
        AssertExists(received);
        RunClipboardCommand();
        AssertNotExists(danglingFile);

        await Verify("someText");

        AssertNotExists(received);
        AssertExists(verified);
    }

    static void RunClipboardCommand()
    {
        foreach (var line in ClipboardCapture.Read().Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
        {
            var command = $"/c {line}";
            Process.Start("cmd.exe", command).WaitForExit();
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
    }
}