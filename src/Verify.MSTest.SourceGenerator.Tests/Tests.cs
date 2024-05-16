namespace VerifyMSTest.SourceGenerator.Tests;

// These tests don't use Verify.SourceGenerator to avoid creating a circular depedency between the repos.

public class Tests
{
    readonly UsesVerifyTestDriver testDriver = new();
    readonly ITestOutputHelper output;

    public Tests(ITestOutputHelper output) => this.output = output;

    async Task VerifyGenerator(GeneratorDriverResults results)
    {
        output.WriteLine($"First run of generators took: {results.FirstRun.TimingInfo.ElapsedTime}");
        output.WriteLine($"Cached re-run of generators took: {results.CachedRun.TimingInfo.ElapsedTime}");

        await Verify(results.FirstRun.RunResult.SelectGeneratedSources());

        // Ensure cachability
        var trackingNames = TrackingNames.GetTrackingNames();
        var trackedSteps1 = results.FirstRun.RunResult.GetTrackedSteps(trackingNames);
        var trackedSteps2 = results.CachedRun.RunResult.GetTrackedSteps(trackingNames);

        trackedSteps2.Keys.ShouldBe(trackedSteps1.Keys);
        foreach (var kvp in trackedSteps1)
        {
            var steps1 = kvp.Value;
            var steps2 = trackedSteps2[kvp.Key];

            steps2.Length.ShouldBe(steps1.Length);
            for (var i = 0; i < steps1.Length; i++)
            {
                var outputs1 = steps1[i].Outputs;
                var outputs2 = steps2[i].Outputs;

                outputs1.Select(o => o.Value).ShouldBe(outputs2.Select(o => o.Value));
                outputs2.Select(o => o.Reason).ShouldAllBe(r => r == IncrementalStepRunReason.Cached || r == IncrementalStepRunReason.Unchanged);
            }
        }
    }

    [Fact]
    public Task NoAttribute()
    {
        var source = """
            public class Foo
            {
            }
            """;

        return VerifyGenerator(testDriver.Run(source));
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

        return VerifyGenerator(testDriver.Run(source));
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

        return VerifyGenerator(testDriver.Run(source));
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

        return VerifyGenerator(testDriver.Run(source));
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

        return VerifyGenerator(testDriver.Run(source));
    }
}
