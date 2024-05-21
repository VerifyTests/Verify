public class NoMatchTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public Task NoAttributes()
    {
        var source = """
            public class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }
}
