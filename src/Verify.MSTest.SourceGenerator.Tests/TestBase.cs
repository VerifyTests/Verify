// These tests don't use Verify.SourceGenerator to avoid creating a circular dependency between the repos.

public abstract class TestBase(ITestOutputHelper output)
{
    private protected TestDriver TestDriver { get; } = new([new UsesVerifyGenerator().AsSourceGenerator()]);
    protected ITestOutputHelper Output { get; } = output;

    private protected async Task VerifyGenerator(GeneratorDriverResults results, IReadOnlyList<string>? expectedDiagnostics = null)
    {
        var first = results.FirstRun;
        Output.WriteLine($"First run of generators took: {first.TimingInfo.ElapsedTime}");
        var cached = results.CachedRun;
        Output.WriteLine($"Cached re-run of generators took: {cached.TimingInfo.ElapsedTime}");

        var diagnostics = results.outputCompilation.GetDiagnostics();
        if (expectedDiagnostics == null ||
            expectedDiagnostics.Count == 0)
        {
            Assert.Empty(diagnostics);
        }
        else
        {
            foreach (var diagnostic in diagnostics)
            {
                Assert.Contains(diagnostic.Id, expectedDiagnostics);
            }
        }

        await Verify(first.RunResult.SelectGeneratedSources());

        // Ensure cachability
        var trackingNames = TrackingNames.AllNames;
        var trackedSteps1 = first.RunResult.GetTrackedSteps(trackingNames);
        var trackedSteps2 = cached.RunResult.GetTrackedSteps(trackingNames);

        Assert.Equal(trackedSteps2.Keys, trackedSteps1.Keys);
        foreach (var (key, steps1) in trackedSteps1)
        {
            var steps2 = trackedSteps2[key];

            Assert.Equal(steps2.Length, steps1.Length);
            for (var i = 0; i < steps1.Length; i++)
            {
                var outputs1 = steps1[i].Outputs;
                var outputs2 = steps2[i].Outputs;

                Assert.Equal(outputs1.Select(_ => _.Value), outputs2.Select(_ => _.Value));
                foreach (var output in outputs2)
                {
                    Assert.True(output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged);
                }
            }
        }
    }
}
