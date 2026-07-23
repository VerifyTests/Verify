// Verify.Tests targets multiple frameworks. The directory convention derives both the received and the
// verified paths from the verified prefix, so, unlike the file convention, the received paths would
// otherwise be shared by every target framework, and the cleanup each run does on init would delete the
// received output of the others. The verified paths have to stay unscoped, so accepting is unaffected.
public class UniqueDirectoryReceivedTests
{
    [Fact]
    public async Task ReceivedIsScopedToRuntimeAndVersion()
    {
        var directory = Path.GetFullPath(CurrentFile.Relative($"{nameof(UniqueDirectoryReceivedTests)}.{nameof(ReceivedIsScopedToRuntimeAndVersion)}"));
        var scoped = Path.Combine(directory, $"target.{Namer.RuntimeAndVersion}.received.txt");
        var unscoped = Path.Combine(directory, "target.received.txt");
        try
        {
            await Assert.ThrowsAsync<VerifyException>(
                () => Verify("Value")
                    .UseUniqueDirectory()
                    .DisableDiff());
            Assert.True(File.Exists(scoped));
            Assert.False(File.Exists(unscoped));
            // the verified name is not scoped, so every framework shares the one snapshot
            Assert.False(File.Exists(Path.Combine(directory, $"target.{Namer.RuntimeAndVersion}.verified.txt")));
        }
        finally
        {
            File.Delete(scoped);
            File.Delete(unscoped);
        }
    }

    [Fact]
    public async Task SplitModeReceivedIsScopedToRuntimeAndVersion()
    {
        var prefix = Path.GetFullPath(CurrentFile.Relative($"{nameof(UniqueDirectoryReceivedTests)}.{nameof(SplitModeReceivedIsScopedToRuntimeAndVersion)}"));
        var scoped = $"{prefix}.{Namer.RuntimeAndVersion}.received";
        var unscoped = $"{prefix}.received";
        try
        {
            await Assert.ThrowsAsync<VerifyException>(
                () => Verify("Value")
                    .UseUniqueDirectory()
                    .UseSplitModeForUniqueDirectory()
                    .DisableDiff());
            Assert.True(File.Exists(Path.Combine(scoped, "target.txt")));
            Assert.False(Directory.Exists(unscoped));
            // the verified directory is not scoped, so every framework shares the one snapshot directory.
            // It is left in place, since the other frameworks may be asserting on it concurrently.
            Assert.True(Directory.Exists($"{prefix}.verified"));
        }
        finally
        {
            IoHelpers.DeleteDirectory(scoped);
            // nothing writes the unscoped directory once the received side is scoped, so this only
            // clears an artifact left by a run against the unfixed behavior
            IoHelpers.DeleteDirectory(unscoped);
        }
    }
}