#region VerifyChecksExpecto
public class VerifyChecksTests
{
    [Tests]
    public static Test verifyChecksTest = Runner.TestCase(
        nameof(verifyChecksTest),
        () => VerifyChecks.Run(typeof(VerifyChecksTests).Assembly));
}
#endregion