[TestFixture]
public class Tests
{
    [TestCase("Value1")]
    public Task UseFileNameWithParam(string arg)
    {
        return Verifier.Verify(arg)
            .UseFileName("UseFileNameWithParam");
    }

    [TestCase("Value1")]
    public Task UseTextForParameters(string arg)
    {
        return Verifier.Verify(arg)
            .UseTextForParameters("TextForParameter");
    }
}