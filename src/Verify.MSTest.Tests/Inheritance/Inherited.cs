namespace TheTests;

[TestClass]
[UsesVerify]
public partial class Inherited : Base
{
    [TestMethod]
    public override Task TestToOverride()
    {
        Trace.WriteLine("");
        return base.TestToOverride();
    }
}