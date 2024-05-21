public class NamespaceTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public Task HasAttributeOnClass()
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
    public Task HasAttributeOnClassWithGenericsInNestedNamespace()
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
}
