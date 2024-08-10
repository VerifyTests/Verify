// disable all test parallelism to avoid test interaction

[assembly: CollectionBehavior(
    CollectionBehavior.CollectionPerAssembly,
    DisableTestParallelization = true,
    MaxParallelThreads = 1)]

public abstract class BaseTest
{
    protected BaseTest() =>
        VerifierSettings.Reset();
}