using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

[TestClass]
public class Tests :
    VerifyBase
{
    [DataTestMethod]
    [DataRow("Value1")]
    public async Task MissingParameter(string arg)
    {
        var exception = await Assert.ThrowsExceptionAsync<AssertFailedException>(() => Verify("Foo"));
        Assert.IsTrue(exception.Message.Contains("requires parameters"));
    }

    [TestMethod]
    public Task VerifyFileContent()
    {
        return VerifyFileContent("Foo", "ext");
    }

    [TestMethod]
    public Task VerifyJson()
    {
        return VerifyJson("{x:y}");
    }
}