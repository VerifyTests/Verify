using System;
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
        var exception = await Assert.ThrowsExceptionAsync<Exception>(() => Verify("Foo"));
        Assert.IsTrue(exception.Message.Contains("requires parameters"));
    }

    [DataTestMethod]
    [DataRow("Value1")]
    public Task UseFileNameWithParam(string arg)
    {
        return Verify(arg)
            .UseFileName("UseFileNameWithParam");
    }

    [DataTestMethod]
    [DataRow("Value1")]
    public Task UseTextForParameters(string arg)
    {
        return Verify(arg)
            .UseTextForParameters("TextForParameter");
    }
}