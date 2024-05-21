public class InheritanceTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public Task HasAttributeOnBaseClass()
    {
        var source = """
            using VerifyMSTest;

            [UsesVerify]
            public partial class Base
            {
            }

            public class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task HasAttributeOnDerivedClass()
    {
        var source = """
            using VerifyMSTest;

            public class Base
            {
            }

            [UsesVerify]
            public partial class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task HasAttributeOnBaseAndDerivedClasses()
    {
        var source = """
            using VerifyMSTest;

            [UsesVerify]
            public partial class Base
            {
            }

            [UsesVerify]
            public partial class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task HasAttributeOnDerivedClassAndPropertyManuallyDefinedInBase()
    {
        var source = """
            using VerifyMSTest;

            public class Base
            {
                public TestContext TestContext { get; set; } = null!;
            }

            [UsesVerify]
            public partial class Derived : Base
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }
}
