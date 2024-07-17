[TestFixtureSource(nameof(FixtureArgs))]
public class TestFixtureSourceAndTestCaseUsage(string arg1, int arg2)
{
    [TestCase("FromTestCase2")]
    public Task Test(string arg3) =>
        Verify(
            new
            {
                arg1,
                arg2,
                arg3
            });

    static object[] FixtureArgs =
    [
        new object[]
        {
            "Value2",
            2
        }
    ];
}