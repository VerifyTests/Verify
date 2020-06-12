using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

[TestClass]
public class Tests
{
    [DataTestMethod]
    [DataRow("Value1")]
    public async Task MissingParameter(string arg)
    {
        var exception = await Assert.ThrowsExceptionAsync<Exception>(() => Verifier.Verify("Foo"));
        Assert.IsTrue(exception.Message.Contains("requires parameters"));
    }
}