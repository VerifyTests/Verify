using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TheTests;

[TestClass]
public class Inherited : Base
{
    [TestMethod]
    public override Task TestToOverride()
    {
        Trace.WriteLine("");
        return base.TestToOverride();
    }
}