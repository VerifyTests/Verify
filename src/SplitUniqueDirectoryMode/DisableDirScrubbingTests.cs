[UsesVerify]
public class SplitUniqueDirectoryModeTests
{
    static SplitUniqueDirectoryModeTests() =>
        VerifierSettings.UseSplitModeForUniqueDirectory();

    [Fact]
    public Task ValueTest() =>
        Verify("value").UseUniqueDirectory();
}