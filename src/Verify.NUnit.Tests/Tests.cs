[TestFixture]
public class Tests
{
    [TestCase("Value1")]
    public Task UseFileNameWithParam(string arg) =>
        Verify(arg)
            .UseFileName("UseFileNameWithParam");

    [TestCase("Value1")]
    public Task UseTextForParameters(string arg) =>
        Verify(arg)
            .UseTextForParameters("TextForParameter");

    [TestCase("Value1", TestName = "CustomName")]
    public Task TestCaseWithName(string arg) =>
        Verify(arg)
            .UseTextForParameters("TextForParameter");

    [TestCase("Value1", TestName = "Custom>Name")]
    public Task TestCaseWithNameAndInvalidChars(string arg) =>
        Verify(arg)
            .UseTextForParameters("TextForParameter");

    [Test]
    public Task WithTargets() =>
        Verify(
            new
            {
                Property = "Value"
            },
            new[]
            {
                new Target("txt", "TextTarget")
            });
}