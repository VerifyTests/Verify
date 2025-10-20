[TestClass]
public partial class ParametersTests
{
    //[TestMethod]
    //[DataRow("1.1")]
    //public async Task Decimal(decimal arg)
    //{
    //    await Verify(arg);
    //}

    [TestMethod]
    [DataRow((float) 1.1)]
    public Task Float(float arg) =>
        Verify(arg);

    [TestMethod]
    [DataRow(1.1d)]
    public Task Double(double arg) =>
        Verify(arg);
}