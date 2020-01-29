using System;
using System.IO;
using System.Threading.Tasks;
using DiffEngine;
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

        var prefix = Path.Combine(SourceDirectory, $"{Context.UniqueTestName}");
        var danglingFile = Path.Combine(SourceDirectory, $"{prefix}.01.verified.{extension}");
        var file = new FilePair(extension, prefix);

        DeleteAll(danglingFile, file.Verified, file.Received);
        File.WriteAllText(danglingFile, "");

        if (hasExistingReceived)
        {
            File.WriteAllText(file.Received, "");
        }

        await InitialVerify(initialTarget, hasMatchingDiffTool, settings, file);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        AssertNotExists(danglingFile);

        await ReVerify(initialTarget, settings, file);

        await InitialVerify(secondTarget, hasMatchingDiffTool, settings, file);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        await ReVerify(secondTarget, settings, file);
    }

    async Task ReVerify(Func<object> target, VerifySettings settings, FilePair pair)
    {
        var command = tool.BuildCommand(pair.Received,pair.Verified);
        ProcessCleanup.Refresh();
        await Verify(target(), settings);
        ProcessCleanup.Refresh();
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
            ProcessCleanup.Refresh();
            AssertProcess(hasMatchingDiffTool, pair);
            if (hasMatchingDiffTool)
            {
                AssertExists(pair.Verified);
            }

            AssertExists(pair.Received);
        }
    }
}