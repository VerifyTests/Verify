using System;
using System.IO;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;

public partial class Tests :
    VerifyBase
{
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
        if (settings.autoVerify)
        {
            await Verify(target(), settings);
            AssertExists(pair.Verified);
        }
        else
        {
            await Throws(() => Verify(target(), settings));
            ProcessCleanup.RefreshCommands();
            AssertProcess(hasMatchingDiffTool, pair);
            if (hasMatchingDiffTool)
            {
                AssertExists(pair.Verified);
            }

            AssertExists(pair.Received);
        }
    }
}