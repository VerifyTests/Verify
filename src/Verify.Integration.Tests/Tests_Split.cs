#if DEBUG

using DiffEngine;

public partial class Tests
{
    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public async Task Split(
        bool hasExistingReceived,
        bool autoVerify)
    {
        var initialTarget = new TypeToSplit("info1", "value1", "value2");
        var secondTarget = new TypeToSplit("info2", "value1.1", "value2.1");
        var settings = new VerifySettings();
        if (autoVerify)
        {
            settings.AutoVerify();
        }

        var concat = ParameterBuilder.Concat(new()
        {
            {"hasExistingReceived", hasExistingReceived},
            {"autoVerify", autoVerify},
        });
        var uniqueTestName = $"Tests.Split_{concat}";

        settings.UseParameters(hasExistingReceived, autoVerify);
        var prefix = Path.Combine(AttributeReader.GetProjectDirectory(),$"{uniqueTestName}.");
        var danglingFile = $"{prefix}03.verified.txt";
        var file0 = new FilePair("txt", $"{prefix}00");
        var file1 = new FilePair("txt", $"{prefix}01");
        var file2 = new FilePair("txt", $"{prefix}02");

        DeleteAll(danglingFile, file0.ReceivedPath, file0.VerifiedPath, file1.VerifiedPath, file1.ReceivedPath, file2.VerifiedPath, file2.ReceivedPath);
        await File.WriteAllTextAsync(danglingFile, "");

        if (hasExistingReceived)
        {
            await File.WriteAllTextAsync(file0.ReceivedPath, "");
            await File.WriteAllTextAsync(file1.ReceivedPath, "");
            await File.WriteAllTextAsync(file2.ReceivedPath, "");
        }

        PrefixUnique.Clear();
        await InitialVerifySplit(initialTarget, true, settings, file0, file1, file2);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        AssertNotExists(danglingFile);

        PrefixUnique.Clear();
        await ReVerifySplit(initialTarget, settings, file0, file1, file2);

        PrefixUnique.Clear();
        await InitialVerifySplit(secondTarget, true, settings, file0, file1, file2);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        PrefixUnique.Clear();
        await ReVerifySplit(secondTarget, settings, file0, file1, file2);
    }

    static async Task ReVerifySplit(TypeToSplit target, VerifySettings settings, FilePair info, FilePair file1, FilePair file2)
    {
        var infoCommand = BuildCommand(info);
        var file1Command = BuildCommand(file1);
        var file2Command = BuildCommand(file2);
        ProcessCleanup.Refresh();
        await Verify(target, settings);
        await Task.Delay(300);
        ProcessCleanup.Refresh();
        AssertProcessNotRunning(infoCommand);
        AssertProcessNotRunning(file1Command);
        AssertProcessNotRunning(file2Command);

        AssertNotExists(info.ReceivedPath);
        AssertExists(info.VerifiedPath);
        AssertNotExists(file1.ReceivedPath);
        AssertExists(file1.VerifiedPath);
        AssertNotExists(file2.ReceivedPath);
        AssertExists(file2.VerifiedPath);
    }

    static async Task InitialVerifySplit(TypeToSplit target, bool hasMatchingDiffTool, VerifySettings settings, FilePair info, FilePair file1, FilePair file2)
    {
        if (settings.autoVerify)
        {
            await Verify(target, settings);
            AssertExists(info.VerifiedPath);
            AssertExists(file1.VerifiedPath);
            AssertExists(file2.VerifiedPath);
        }
        else
        {
            await Throws(() => Verify(target, settings));
            ProcessCleanup.Refresh();
            AssertProcess(hasMatchingDiffTool, info, file1, file2);
            if (hasMatchingDiffTool)
            {
                AssertExists(info.VerifiedPath);
                AssertExists(file1.VerifiedPath);
                AssertExists(file2.VerifiedPath);
            }

            AssertExists(info.ReceivedPath);
            AssertExists(file1.ReceivedPath);
            AssertExists(file2.ReceivedPath);
        }
    }
}
#endif