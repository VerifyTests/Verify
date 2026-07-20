public class SingleTfmTests
{
    [Fact]
    public async Task Simple()
    {
        var verified = CurrentFile.Relative("SingleTfmTests.Simple.verified.txt");
        var received = CurrentFile.Relative("SingleTfmTests.Simple.received.txt");
        File.Delete(verified);
        File.Delete(received);
        var settings = new VerifySettings();
        settings.DisableDiff();
        try
        {
            await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));
            Assert.True(File.Exists(received));
        }
        finally
        {
            File.Delete(verified);
            File.Delete(received);
        }
    }

    // With a single target framework the received file carries the same uniqueness as the verified
    // file, so the two names match and tooling can pair a received file with its verified file. When
    // multiple frameworks are targeted the received file uses the runtime and version instead, which
    // Verify.Tests covers via DanglingFiles.
    [Fact]
    public Task ReceivedUsesRuntime() =>
        AssertReceivedIsSuffixed(
            "SingleTfmTests.ReceivedUsesRuntime",
            Namer.Runtime,
            _ => _.UniqueForRuntime());

    [Fact]
    public Task ReceivedUsesRuntimeAndVersion() =>
        AssertReceivedIsSuffixed(
            "SingleTfmTests.ReceivedUsesRuntimeAndVersion",
            Namer.RuntimeAndVersion,
            _ => _.UniqueForRuntimeAndVersion());

    [Fact]
    public Task ReceivedUsesRuntimeForFileName() =>
        AssertReceivedIsSuffixed(
            "CustomFileName",
            Namer.Runtime,
            _ =>
            {
                _.UseFileName("CustomFileName");
                _.UniqueForRuntime();
            });

    static async Task AssertReceivedIsSuffixed(string prefix, string suffix, Action<VerifySettings> configure)
    {
        var verified = CurrentFile.Relative($"{prefix}.{suffix}.verified.txt");
        var received = CurrentFile.Relative($"{prefix}.{suffix}.received.txt");
        var unsuffixed = CurrentFile.Relative($"{prefix}.received.txt");
        File.Delete(verified);
        File.Delete(received);
        File.Delete(unsuffixed);
        var settings = new VerifySettings();
        settings.DisableDiff();
        configure(settings);
        try
        {
            await Assert.ThrowsAsync<VerifyException>(() => Verify("value", settings));
            Assert.True(File.Exists(received));
            Assert.False(File.Exists(unsuffixed));
        }
        finally
        {
            File.Delete(verified);
            File.Delete(received);
            File.Delete(unsuffixed);
        }
    }
}