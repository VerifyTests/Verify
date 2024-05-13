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

        var results = TestDriver
            .Run(source)
            .Results
            .SelectMany(grr => grr.GeneratedSources)
            .Select(gs => (gs.HintName, gs.SourceText.ToString()))
            .SingleOrDefault();

        return Verify(results);
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

        var results = TestDriver
            .Run(source)
            .Results
            .SelectMany(grr => grr.GeneratedSources)
            .Select(gs => (gs.HintName, gs.SourceText.ToString()))
            .SingleOrDefault();

        return Verify(results);
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

        var results = TestDriver
            .Run(source)
            .Results
            .SelectMany(grr => grr.GeneratedSources)
            .Select(gs => (gs.HintName, gs.SourceText.ToString()))
            .SingleOrDefault();

        return Verify(results);
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

        var results = TestDriver
            .Run(source)
            .Results
            .SelectMany(grr => grr.GeneratedSources)
            .Select(gs => (gs.HintName, gs.SourceText.ToString()))
            .SingleOrDefault();

        return Verify(results);
    }
}
