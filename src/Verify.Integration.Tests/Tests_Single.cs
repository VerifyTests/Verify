#if DEBUG
using System;
using System.IO;
using System.Threading.Tasks;
using DiffEngine;
using VerifyTests;
using VerifyXunit;
using Xunit;

public partial class Tests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task Text(
        bool hasExistingReceived,
        bool autoVerify)
    {
        var uniqueTestName = TestNameBuilder.GetUniqueTestName(
            "Tests_Single",
            Info.OfMethod<Tests>("Text"),
            new object[]{hasExistingReceived, autoVerify});
        VerifySettings settings = new();
        settings.UseParameters(hasExistingReceived, autoVerify);
        await RunTest(
            "txt",
            () => "someText",
            () => "someOtherText",
            hasMatchingDiffTool: true,
            hasExistingReceived,
            autoVerify,
            settings,
            uniqueTestName);
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
    public async Task Stream(
        bool hasMatchingDiffTool,
        bool hasExistingReceived,
        bool autoVerify)
    {
        var extension = hasMatchingDiffTool ? "knownBin" : "unknownBin";

        var uniqueTestName = TestNameBuilder.GetUniqueTestName(
            "Tests_Single",
            Info.OfMethod<Tests>("Stream"),
            new object[] {hasMatchingDiffTool, hasExistingReceived, autoVerify});
        VerifySettings settings = new();
        settings.UseParameters(hasMatchingDiffTool, hasExistingReceived, autoVerify);
        await RunTest(
            extension: extension,
            () => new MemoryStream(new byte[] {1}),
            () => new MemoryStream(new byte[] {2}),
            hasMatchingDiffTool,
            hasExistingReceived,
            autoVerify,
            settings,
            uniqueTestName);
    }

    async Task RunTest(
        string extension,
        Func<object> initialTarget,
        Func<object> secondTarget,
        bool hasMatchingDiffTool,
        bool hasExistingReceived,
        bool autoVerify,
        VerifySettings settings,
        string uniqueTestName)
    {
        settings.UseExtension(extension);
        if (autoVerify)
        {
            settings.AutoVerify();
        }
        var prefix = Path.Combine(SourceDirectory, uniqueTestName);
        var danglingFile = Path.Combine(SourceDirectory, $"{prefix}.01.verified.{extension}");
        FilePair file = new(extension, prefix);

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

    static async Task ReVerify(Func<object> target, VerifySettings settings, FilePair pair)
    {
        var command = BuildCommand(pair);
        ProcessCleanup.Refresh();
        await Verifier.Verify(target(), settings);
        await Task.Delay(300);
        ProcessCleanup.Refresh();
        AssertProcessNotRunning(command);

        AssertNotExists(pair.Received);
        AssertExists(pair.Verified);
    }

    static async Task InitialVerify(Func<object> target, bool hasMatchingDiffTool, VerifySettings settings, FilePair pair)
    {
        if (settings.autoVerify)
        {
            await Verifier.Verify(target(), settings);
            AssertExists(pair.Verified);
        }
        else
        {
            await Throws(() => Verifier.Verify(target(), settings));
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
#endif