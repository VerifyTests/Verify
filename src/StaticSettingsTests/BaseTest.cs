using Xunit.Sdk;
using Xunit.v3;

// disable all test parallelism to avoid test interaction

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
[assembly: Parallelization(Mode = ParallelMode.None, MaxThreads = 1)]

public abstract class BaseTest
{
    static bool buildServerDetected;

    static BaseTest() => buildServerDetected = DiffEngine.BuildServerDetector.Detected;

    protected BaseTest()
    {
        DiffEngine.BuildServerDetector.Detected = buildServerDetected;
        VerifierSettings.Reset();
        CombinationSettings.Reset();
        DerivePathInfo(PathInfo.DeriveDefault);
    }
}