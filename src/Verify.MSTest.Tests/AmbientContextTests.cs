// The test class is resolved by name from TestContext.Current, so these cover the two shapes
// where that name is not simply the class the method is written in.
[TestClass]
public class AmbientContextTests
{
    // A deferred continuation: the verification runs after the test method has already returned
    // the task, so it only passes while the ambient context still flows.
    [TestMethod]
    public async Task AfterYield()
    {
        await Task.Yield();
        await Verify("YieldedValue");
    }
}

// The test method is declared on the base, but the snapshot has to be named for the derived class.
public abstract class AmbientBase
{
    [TestMethod]
    public Task Inherited() =>
        Verify("InheritedValue");
}

[TestClass]
public class AmbientDerived : AmbientBase;
