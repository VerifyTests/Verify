// Without this the source generator never plumbs the TestContext through, and every verification
// fails with "TestContext is null" before it can produce a snapshot. Applied to the class rather
// than the assembly, since an assembly wide attribute also reaches the static Cleanup class, where
// the generated TestContext property does not compile.
[UsesVerify]
[TestClass]
public partial class Tests
{
    [TestMethod]
    public Task Simple() =>
        Verify("Foo");

    [TestMethod]
    public Task IncorrectCase() =>
        Verify("Foo");
}