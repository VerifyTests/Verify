namespace Verify.MSTest.SourceGenerator.Tests;

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
            .Select(gs => gs.SourceText.ToString())
            .ToArray();

        // TODO: Why is static using not working?
        return Verifier.Verify(results);
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
            .Select(gs => gs.SourceText.ToString())
            .ToArray();

        // TODO: Why is static using not working?
        return Verifier.Verify(results);
    }

    [Fact]
    public Task HasAttributeInNamespace()
    {
        var source = """
            using VerifyMSTest;

            namespace Foo

            [UsesVerify]
            public class Bar
            {
            }
            """;

        var results = TestDriver
            .Run(source)
            .Results
            .SelectMany(grr => grr.GeneratedSources)
            .Select(gs => gs.SourceText.ToString())
            .ToArray();

        // TODO: Why is static using not working?
        return Verifier.Verify(results);
    }
}
