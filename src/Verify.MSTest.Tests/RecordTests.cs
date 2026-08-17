// A record is a class, so it is a legal test class. Without generator support the
// TestContext property is never generated and this fails with "TestContext is null".
[TestClass]
public partial record RecordTests
{
    [TestMethod]
    public Task ShouldPass() =>
        Verify("RecordValue");
}
