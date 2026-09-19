// A record is a class, so it is a legal test class.
[TestClass]
public record RecordTests
{
    [TestMethod]
    public Task ShouldPass() =>
        Verify("RecordValue");
}
