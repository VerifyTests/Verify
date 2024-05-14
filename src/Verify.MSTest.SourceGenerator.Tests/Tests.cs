namespace VerifyMSTest.SourceGenerator.Tests;

// These tests don't use Verify.SourceGenerator to avoid creating a circular depedency between the repos.

public class Tests
{
    [Fact]
    public Task NoAttribute()
    {
        var source = """
            public class Foo
            {
            }
            """;

        return Verify(TestDriver.Run(source).SelectGeneratedSources());
    }

    [Fact]
    public Task HasAttributeInGlobalNamespace()
    {
        var source = """
            using VerifyMSTest;

            [UsesVerify]
            public class Foo
            {
            }
            """;

        return Verify(TestDriver.Run(source).SelectGeneratedSources());
    }

    [Fact]
    public Task HasAttributeInNamespace()
    {
        var source = """
            using VerifyMSTest;

            namespace Foo;

            [UsesVerify]
            public class Bar
            {
            }
            """;

        return Verify(TestDriver.Run(source).SelectGeneratedSources());
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

        return Verify(TestDriver.Run(source).SelectGeneratedSources());
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

        return Verify(TestDriver.Run(source).SelectGeneratedSources());
    }
}
