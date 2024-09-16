namespace MyNamespace;

[TestFixtureSource(nameof(FixtureArgs))]
public class TestFixtureSourceUsageWithNamespace(string arg1, int arg2)
{
    [Test]
    public Task Test() =>
        Verify(new
        {
            arg1,
            arg2
        });

    static object[] FixtureArgs =
    [
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
    ];
}