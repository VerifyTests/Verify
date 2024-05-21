public class Tests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public Task NoAttribute()
    {
        var source = """
            public class Foo
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task HasAttributeInGlobalNamespace()
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

    [Fact]
    public Task HasAttributeInNamespace()
    {
        var source = """
            using VerifyMSTest;

            namespace Foo;

            [UsesVerify]
            public partial class Bar
            {
            }
            """;

        return VerifyGenerator(TestDriver.Run(source));
    }

    [Fact]
    public Task HasAttributeInNestedNamespaceAndClassWithGenerics()
    {
        var source = """
            using VerifyMSTest;

            namespace A
            {
                namespace B
                {
                    public partial class C<T> where T : new()
                    {
                        public partial class D
                        {
                            [UsesVerify]
                            public partial class TestClass1<U>
                            {
                            }

                            [UsesVerify]
                            public partial class TestClass2
                            {
                            }
                        }
                    }
                }
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
}
