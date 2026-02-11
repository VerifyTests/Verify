// These tests don't use Verify.SourceGenerator to avoid creating a circular dependency between the repos.

[UsesVerify]
public abstract partial class TestBase
{
    private protected TestDriver TestDriver { get; } = new([new UsesVerifyGenerator().AsSourceGenerator()]);

    private protected async Task VerifyGenerator(GeneratorDriverResults results, IReadOnlyList<string>? expectedDiagnostics = null)
    {
        var first = results.FirstRun;
        Console.WriteLine($"First run of generators took: {first.TimingInfo.ElapsedTime}");
        var cached = results.CachedRun;
        Console.WriteLine($"Cached re-run of generators took: {cached.TimingInfo.ElapsedTime}");

        var diagnostics = results.outputCompilation.GetDiagnostics();
        if (expectedDiagnostics == null ||
            expectedDiagnostics.Count == 0)
        {
            Assert.AreEqual(0, diagnostics.Length);
        }
        else
        {
            foreach (var diagnostic in diagnostics)
            {
                CollectionAssert.Contains(expectedDiagnostics.ToList(), diagnostic.Id);
            }
        }

        await Verify(first.RunResult.SelectGeneratedSources());

        // Ensure cachability
        var trackingNames = TrackingNames.AllNames;
        var trackedSteps1 = first.RunResult.GetTrackedSteps(trackingNames);
        var trackedSteps2 = cached.RunResult.GetTrackedSteps(trackingNames);

        CollectionAssert.AreEqual(trackedSteps1.Keys.ToList(), trackedSteps2.Keys.ToList());
        foreach (var (key, steps1) in trackedSteps1)
        {
            var steps2 = trackedSteps2[key];

            Assert.AreEqual(steps1.Length, steps2.Length);
            for (var i = 0; i < steps1.Length; i++)
            {
                var outputs1 = steps1[i].Outputs;
                var outputs2 = steps2[i].Outputs;

                CollectionAssert.AreEqual(outputs1.Select(_ => _.Value).ToList(), outputs2.Select(_ => _.Value).ToList());
                foreach (var output in outputs2)
                {
                    Assert.IsTrue(output.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged);
                }
            }
        }
    }
}
