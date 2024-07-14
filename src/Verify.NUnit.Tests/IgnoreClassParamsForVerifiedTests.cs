[TestFixture("1")]
[TestFixture("2")]
public class IgnoreClassParamsForVerifiedTests(string arg)
{
    /// <summary>
    /// IgnoreParametersForVerified only works on method params, not class params.
    /// This test shows a workaround, using a combination of UseTypeName and DisableRequireUniquePrefix, that removes the class params from the output file name.
    /// </summary>
    [Test]
    public async Task WithClassParamsIgnored() => await Verify("Same value for any argument")
        .UseTypeName(GetType().Name)
        .DisableRequireUniquePrefix();

    private void SomeDummyConsumingTheArg() => Console.WriteLine(arg);
}