[TestClass]
[UsesVerify]
public class Inherited : Base
{
    [TestMethod]
    public override Task TestToOverride()
    {
        Trace.WriteLine("");
        return base.TestToOverride();
    }
}