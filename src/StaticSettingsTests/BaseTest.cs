// disable all test parallelism to avoid test interaction

[assembly: CollectionBehavior(
    CollectionBehavior.CollectionPerAssembly,
    DisableTestParallelization = true,
    MaxParallelThreads = 1)]

public abstract class BaseTest
{
    static bool buildServerDetected;

    static BaseTest() => buildServerDetected = DiffEngine.BuildServerDetector.Detected;

    protected BaseTest()
    {
        DiffEngine.BuildServerDetector.Detected = buildServerDetected;
        VerifierSettings.Reset();
        CombinationSettings.Reset();
    }
}