// disable all test parallelism to avoid test interaction

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

[UsesVerify]
public abstract class BaseTest
{
    protected BaseTest() =>
        VerifierSettings.Reset();
}