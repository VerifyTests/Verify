[TestFixture]
public class IgnoreLocalParamsForVerifiedTests
{
    [Test]
    [TestCase("1")]
    [TestCase("2")]
    public async Task WithLocalParamsIgnored(string arg) => await Verify("Same value for any argument").IgnoreParametersForVerified(arg);
}