[TestClass]
public partial class NamespaceTests : TestBase
{
    [TestMethod]
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

    [TestMethod]
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

                            [UsesVerify]
                            public partial class TestClass3<T1, T2>
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
