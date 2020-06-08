#if DEBUG
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DiffEngine;
using Verify;
using VerifyXunit;
using Xunit;

public partial class Tests
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
            autoVerify,
            Info.OfMethod<Tests>("Text",));
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
            autoVerify,
            Info.OfMethod<Tests>("Stream"));
    }

    async Task RunTest(
        string extension,
        Func<object> initialTarget,
        Func<object> secondTarget,
        bool hasMatchingDiffTool,
        bool hasExistingReceived,
        bool autoVerify,
        MethodInfo caller)
    {
        var settings = new VerifySettings();
        settings.UseExtension(extension);
        if (autoVerify)
        {
            settings.AutoVerify();
        }

        var uniqueTestName = TestNameBuilder.GetUniqueTestName(
            typeof(Tests),
            caller,
            new object[] {hasMatchingDiffTool, hasExistingReceived, autoVerify});
        var prefix = Path.GetFullPath(uniqueTestName);
        var danglingFile = Path.GetFullPath($"{prefix}.01.verified.{extension}");
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
        var command = BuildCommand(pair);
        ProcessCleanup.Refresh();
        await Verifier.Verify(target(), settings);
        ProcessCleanup.Refresh();
        AssertProcessNotRunning(command);

        AssertNotExists(pair.Received);
        AssertExists(pair.Verified);
    }

    async Task InitialVerify(Func<object> target, bool hasMatchingDiffTool, VerifySettings settings, FilePair pair)
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