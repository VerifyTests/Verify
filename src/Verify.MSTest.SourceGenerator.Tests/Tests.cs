// These tests don't use Verify.SourceGenerator to avoid creating a circular dependency between the repos.

public class Tests(ITestOutputHelper output)
{
    readonly UsesVerifyTestDriver testDriver = new();

    async Task VerifyGenerator(GeneratorDriverResults results)
    {
        var first = results.FirstRun;
        output.WriteLine($"First run of generators took: {first.TimingInfo.ElapsedTime}");
        var cached = results.CachedRun;
        output.WriteLine($"Cached re-run of generators took: {cached.TimingInfo.ElapsedTime}");

        await Verify(first.RunResult.SelectGeneratedSources());

        // Ensure cachability
        var trackingNames = TrackingNames.GetTrackingNames();
        var trackedSteps1 = first.RunResult.GetTrackedSteps(trackingNames);
        var trackedSteps2 = cached.RunResult.GetTrackedSteps(trackingNames);

        trackedSteps2.Keys.ShouldBe(trackedSteps1.Keys);
        foreach (var (key, steps1) in trackedSteps1)
        {
            var steps2 = trackedSteps2[key];

            steps2.Length.ShouldBe(steps1.Length);
            for (var i = 0; i < steps1.Length; i++)
            {
                var outputs1 = steps1[i].Outputs;
                var outputs2 = steps2[i].Outputs;

                outputs1.Select(_ => _.Value)
                    .ShouldBe(outputs2.Select(_ => _.Value));
                outputs2.Select(_ => _.Reason)
                    .ShouldAllBe(_ => _ == IncrementalStepRunReason.Cached ||
                                      _ == IncrementalStepRunReason.Unchanged);
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
