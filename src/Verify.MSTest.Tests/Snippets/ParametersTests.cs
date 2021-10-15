using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

namespace TheTests;

[TestClass]
public class ParametersTests :
    VerifyBase
{
    //[DataTestMethod]
    //[DataRow("1.1")]
    //public async Task Decimal(decimal arg)
    //{
    //    await Verify(arg)
    //        .UseParameters(arg);
    //}

    [DataTestMethod]
    [DataRow((float) 1.1)]
    public async Task Float(float arg)
    {
        await Verify(arg)
            .UseParameters(arg);
    }

    [DataTestMethod]
    [DataRow(1.1d)]
    public async Task Double(double arg)
    {
        await Verify(arg)
            .UseParameters(arg);
    }
}