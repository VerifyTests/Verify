#if DEBUG

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
    public Task Split(
        bool hasExistingReceived,
        bool autoVerify)
    {
        return RunSplitTest(
            new TypeToSplit("info1","value1","value2"),
            new TypeToSplit("info2","value1.1","value2.1"),
            hasMatchingDiffTool: true,
            hasExistingReceived,
            autoVerify);
    }

    async Task RunSplitTest(
        TypeToSplit initialTarget,
        TypeToSplit secondTarget,
        bool hasMatchingDiffTool,
        bool hasExistingReceived,
        bool autoVerify)
    {
        var settings = new VerifySettings();
        if (autoVerify)
        {
            settings.AutoVerify();
        }

        var prefix = Path.Combine(SourceDirectory, $"{Context.UniqueTestName}.");
        var danglingFile = $"{prefix}03.verified.txt";
        var info = new FilePair("txt", $"{prefix}info");
        var file1 = new FilePair("txt", $"{prefix}00");
        var file2 = new FilePair("txt", $"{prefix}01");

        DeleteAll(danglingFile, info.Received, info.Verified, file1.Verified, file1.Received, file2.Verified, file2.Received);
        File.WriteAllText(danglingFile, "");

        if (hasExistingReceived)
        {
            File.WriteAllText(info.Received, "");
            File.WriteAllText(file1.Received, "");
            File.WriteAllText(file2.Received, "");
        }


        await InitialVerifySplit(initialTarget, hasMatchingDiffTool, settings, info, file1, file2);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        AssertNotExists(danglingFile);

        await ReVerifySplit(initialTarget, settings, info, file1, file2);

        await InitialVerifySplit(secondTarget, hasMatchingDiffTool, settings, info, file1, file2);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        await ReVerifySplit(secondTarget, settings, info, file1, file2);
    }

    async Task ReVerifySplit(TypeToSplit target, VerifySettings settings, FilePair info, FilePair file1, FilePair file2)
    {
        var infoCommand = tool.BuildCommand(info.Received, info.Verified);
        var file1Command = tool.BuildCommand(file1.Received, file1.Verified);
        var file2Command = tool.BuildCommand(file2.Received, file2.Verified);
        ProcessCleanup.Refresh();
        await Verify(target, settings);
        ProcessCleanup.Refresh();
        AssertProcessNotRunning(infoCommand);
        AssertProcessNotRunning(file1Command);
        AssertProcessNotRunning(file2Command);

        AssertNotExists(info.Received);
        AssertExists(info.Verified);
        AssertNotExists(file1.Received);
        AssertExists(file1.Verified);
        AssertNotExists(file2.Received);
        AssertExists(file2.Verified);
    }

    async Task InitialVerifySplit(TypeToSplit target, bool hasMatchingDiffTool, VerifySettings settings, FilePair info, FilePair file1, FilePair file2)
    {
        if (settings.autoVerify)
        {
            await Verify(target, settings);
            AssertExists(info.Verified);
            AssertExists(file1.Verified);
            AssertExists(file2.Verified);
        }
        else
        {
            await Throws(() => Verify(target, settings));
            ProcessCleanup.Refresh();
            AssertProcess( hasMatchingDiffTool, info,file1, file2);
            if (hasMatchingDiffTool)
            {
                AssertExists(info.Verified);
                AssertExists(file1.Verified);
                AssertExists(file2.Verified);
            }

            AssertExists(info.Received);
            AssertExists(file1.Received);
            AssertExists(file2.Received);
        }
    }
}
#endif