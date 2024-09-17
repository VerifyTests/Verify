[MethodDataSource(nameof(FixtureArgs), UnfoldTuple = true)]
public class TestFixtureSourceAndTestCaseUsage(string arg1, int arg2)
{
    [Test]
    [Arguments("FromTestCase2")]
    public Task Test(string arg3) =>
        Verify(
            new
            {
                arg1,
                arg2,
                arg3
            });

    public static (string, int) FixtureArgs() =>
        ("Value", 2);
}