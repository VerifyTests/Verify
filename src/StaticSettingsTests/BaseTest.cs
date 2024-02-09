// disable all test parallelism to avoid test interaction

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

public abstract class BaseTest
{
    protected BaseTest() =>
        VerifierSettings.Reset();
}