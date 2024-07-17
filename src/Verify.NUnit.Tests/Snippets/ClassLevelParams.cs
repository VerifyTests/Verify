#pragma warning disable CS9113 // Parameter is unread.
[TestFixture("1")]
//[TestFixture("2")]
public class ClassLevelParams(string arg1)
{
    [Test]
    public Task Simple() => Verify("Value");

    [TestCase("3")]
    //[TestCase("4")]
    public Task WithMethodLevel(string arg2) => Verify("Value");

    [TestCase("3")]
    [TestCase("4")]
    public Task IgnoreParameters(string arg2) =>
        Verify("Value")
            .IgnoreParametersForVerified();
}