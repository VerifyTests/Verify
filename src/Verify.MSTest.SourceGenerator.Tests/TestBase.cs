// These tests don't use Verify.SourceGenerator to avoid creating a circular dependency between the repos.

public abstract class TestBase(ITestOutputHelper output)
{
    private protected TestDriver TestDriver { get; } = new([new UsesVerifyGenerator().AsSourceGenerator()]);
    protected ITestOutputHelper Output { get; } = output;

    private protected async Task VerifyGenerator(GeneratorDriverResults results, IEnumerable<string>? expectedDiagnostics = null)
    {
        var first = results.FirstRun;
        Output.WriteLine($"First run of generators took: {first.TimingInfo.ElapsedTime}");
        var cached = results.CachedRun;
        Output.WriteLine($"Cached re-run of generators took: {cached.TimingInfo.ElapsedTime}");

        expectedDiagnostics ??= [];
        results.outputCompilation.GetDiagnostics().ShouldAllBe(_ => expectedDiagnostics.Contains(_.Id));

        await Verify(first.RunResult.SelectGeneratedSources());

        // Ensure cachability
        var trackingNames = TrackingNames.AllNames;
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
}
