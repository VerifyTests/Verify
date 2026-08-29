public class ParamsArrayTests
{
    // TestMethodArguments holds the raw pre-binding arguments, which TUnit bundles into
    // the params array at invocation time. So the argument count does not match the
    // parameter count and the arguments cannot be used for snapshot naming.
    [Test]
    [Arguments(1, 2, 3)]
    public Task ParamsArray(params int[] values) =>
        Verify(values.Sum());
}
