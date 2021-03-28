#if DEBUG

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
    public async Task Split(
        bool hasExistingReceived,
        bool autoVerify)
    {
        TypeToSplit initialTarget = new("info1", "value1", "value2");
        TypeToSplit secondTarget = new("info2", "value1.1", "value2.1");
        VerifySettings settings = new();
        if (autoVerify)
        {
            settings.AutoVerify();
        }

        var method = GetType().GetMethod("Split")!;

        var concat = ParameterBuilder.Concat(method, new object[] {hasExistingReceived, autoVerify});
        var uniqueTestName = $"Tests.Split_{concat}";

        settings.UseParameters(hasExistingReceived, autoVerify);
        var prefix = Path.Combine(AttributeReader.GetProjectDirectory(),$"{uniqueTestName}.");
        var danglingFile = $"{prefix}03.verified.txt";
        FilePair file0 = new("txt", $"{prefix}00");
        FilePair file1 = new("txt", $"{prefix}01");
        FilePair file2 = new("txt", $"{prefix}02");

        DeleteAll(danglingFile, file0.Received, file0.Verified, file1.Verified, file1.Received, file2.Verified, file2.Received);
        await File.WriteAllTextAsync(danglingFile, "");

        if (hasExistingReceived)
        {
            await File.WriteAllTextAsync(file0.Received, "");
            await File.WriteAllTextAsync(file1.Received, "");
            await File.WriteAllTextAsync(file2.Received, "");
        }

        FileNameBuilder.ClearPrefixList();
        await InitialVerifySplit(initialTarget, true, settings, file0, file1, file2);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        AssertNotExists(danglingFile);

        FileNameBuilder.ClearPrefixList();
        await ReVerifySplit(initialTarget, settings, file0, file1, file2);

        FileNameBuilder.ClearPrefixList();
        await InitialVerifySplit(secondTarget, true, settings, file0, file1, file2);

        if (!autoVerify)
        {
            RunClipboardCommand();
        }

        FileNameBuilder.ClearPrefixList();
        await ReVerifySplit(secondTarget, settings, file0, file1, file2);
    }

    static async Task ReVerifySplit(TypeToSplit target, VerifySettings settings, FilePair info, FilePair file1, FilePair file2)
    {
        var infoCommand = BuildCommand(info);
        var file1Command = BuildCommand(file1);
        var file2Command = BuildCommand(file2);
        ProcessCleanup.Refresh();
        await Verifier.Verify(target, settings);
        await Task.Delay(300);
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

    static async Task InitialVerifySplit(TypeToSplit target, bool hasMatchingDiffTool, VerifySettings settings, FilePair info, FilePair file1, FilePair file2)
    {
        if (settings.autoVerify)
        {
            await Verifier.Verify(target, settings);
            AssertExists(info.Verified);
            AssertExists(file1.Verified);
            AssertExists(file2.Verified);
        }
        else
        {
            await Throws(() => Verifier.Verify(target, settings));
            ProcessCleanup.Refresh();
            AssertProcess(hasMatchingDiffTool, info, file1, file2);
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