#pragma warning disable CS9113 // Parameter is unread.
[Arguments("1")]
[Arguments("2")]
public class ClassLevelParams(string arg1)
{
    [Test]
    public Task Simple() => Verify("Value");

    [Test]
    [Arguments("3")]
    [Arguments("4")]
    public Task WithMethodLevel(string arg2) => Verify("Value");

    [Test]
    [Arguments("3")]
    [Arguments("4")]
    public Task IgnoreParameters(string arg2) =>
        Verify("Value")
            .IgnoreParametersForVerified();
}