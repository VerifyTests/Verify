#pragma warning disable CS9113 // Parameter is unread.
[TestFixture("1")]
[TestFixture("2")]
public class ClassLevelParams(string arg1)
{
    [Test]
    public Task Simple() => Verify("Value");

    [TestCase("3")]
    [TestCase("4")]
    public Task WithMethodLevel(string arg2) => Verify("Value");

    [TestCase("3")]
    [TestCase("4")]
    public Task IgnoreParameters(string arg2) =>
        Verify("Value")
            .IgnoreParametersForVerified();

    [TestCase("3")]
    [TestCase("4")]
    public Task IgnoreClassLevelParametersOnly(string arg2) =>
        Verify(arg2)
            .IgnoreParameters(nameof(arg1));

    [TestCase("3.1", "3.2")]
    [TestCase("4.1", "4.2")]
    public Task IgnoreMethodParameterOnly(string arg2, string arg3) =>
        Verify($"{arg1}_{arg3}")
            .IgnoreParameters(nameof(arg2));

    [TestCase("3")]
    [TestCase("4")]
    public void IgnoreParameters_ThrowsOnInvalidParameterName(string arg2) =>
        ThrowsAsync<Exception>(() =>
            Verify(arg2)
                .IgnoreParameters("XYZ")
        );
}