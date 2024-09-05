#region VerifyChecksMSTest
[TestClass]
public partial class VerifyChecksTests
{
    [TestMethod]
    public Task Run() =>
        VerifyChecks.Run();
}
#endregion