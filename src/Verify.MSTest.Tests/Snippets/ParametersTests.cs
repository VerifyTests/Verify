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
    public Task Float(float arg) =>
        Verify(arg)
            .UseParameters(arg);

    [DataTestMethod]
    [DataRow(1.1d)]
    public Task Double(double arg) =>
        Verify(arg)
            .UseParameters(arg);
}