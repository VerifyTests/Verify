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

        // A test that queued with the switch on leaves a record in obj, and the next verification
        // with the switch off would retire it through whatever owns the queue on this machine
        if (Directory.Exists(InlineSwitchRecordsDirectory))
        {
            Directory.Delete(InlineSwitchRecordsDirectory, true);
        }
    }

    protected static string InlineSwitchRecordsDirectory =>
        Path.Combine(
            AttributeReader.GetIntermediateDirectory(typeof(BaseTest).Assembly),
            InlineSwitchRecords.DirectoryName);
}