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
        var infoPair = new FilePair("txt", $"{prefix}info");
        var filePair0 = new FilePair("txt", $"{prefix}00");
        var filePair1 = new FilePair("txt", $"{prefix}01");

        DeleteAll(danglingFile, infoPair.Received, infoPair.Verified, filePair0.Verified, filePair0.Received, filePair1.Verified, filePair1.Received);
        File.WriteAllText(danglingFile, "");

        if (hasExistingReceived)
        {
            File.WriteAllText(infoPair.Received, "");
            File.WriteAllText(filePair0.Received, "");
            File.WriteAllText(filePair1.Received, "");
        }


        await InitialVerifySplit(initialTarget, hasMatchingDiffTool, settings, infoPair, filePair0, filePair1);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        AssertNotExists(danglingFile);

        await ReVerifySplit(initialTarget, settings, infoPair, filePair0, filePair1);

        await InitialVerifySplit(secondTarget, hasMatchingDiffTool, settings, infoPair, filePair0, filePair1);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        await ReVerifySplit(secondTarget, settings, infoPair, filePair0, filePair1);
    }

    async Task ReVerifySplit(TypeToSplit target, VerifySettings settings, FilePair info, FilePair file1, FilePair file2)
    {
        var infoCommand = tool.BuildCommand(info);
        var file1Command = tool.BuildCommand(file1);
        var file2Command = tool.BuildCommand(file2);
        ProcessCleanup.RefreshCommands();
        await Verify(target, settings);
        ProcessCleanup.RefreshCommands();
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
            ProcessCleanup.RefreshCommands();
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