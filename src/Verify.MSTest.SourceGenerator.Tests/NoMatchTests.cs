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

    [Fact]
    public Task AttributeFromWrongNamespace()
    {
        var source = """
            [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
            public sealed class UsesVerifyAttribute : Attribute;

            [UsesVerify]
            public class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }
}
