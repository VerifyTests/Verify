using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;
[TestClass]
public class VerifyTextSample
{
    [TestMethod]
    public Task Simple()
    {
        return Verifier.Verify("Foo");
    }
}