public class GlobalNamespaceTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public Task HasAttributeOnClass()
    {
        var source = """
            using VerifyMSTest;

            [UsesVerify]
            public partial class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }
}
