[TestFixtureSource(nameof(FixtureArgs))]
public class TestFixtureSourceUsage
{
    string arg1;
    int arg2;

    public TestFixtureSourceUsage(string arg1, int arg2)
    {
        this.arg1 = arg1;
        this.arg2 = arg2;
    }

    [Test]
    public Task Test() =>
        Verify(new
        {
            arg1,
            arg2
        });

    static object[] FixtureArgs =
    {
        new object[]
        {
            "Value1",
            1
        },
        new object[]
        {
            "Value2",
            2
        }
    };
}