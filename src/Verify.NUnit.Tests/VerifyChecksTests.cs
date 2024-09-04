[TestFixture]
public class VerifyChecksTests
{
    [Test]
    public Task RunVerifyChecks() =>
        VerifyChecks.Run();
}