[UsesVerify]
public class Tests
{
    [Theory]
    [InlineData("Value1")]
    public async Task MissingParameter(string arg)
    {
        var exception = await Assert.ThrowsAsync<Exception>(() => Verify("Foo"));
        Assert.Contains("requires parameters", exception.Message);
    }

    [Theory]
    [MemberData(nameof(GetData))]
    public Task UseFileNameWithParam(string arg) =>
        Verify(arg)
            .UseFileName("UseFileNameWithParam");

    public static IEnumerable<object[]> GetData()
    {
        yield return new object[] {"Value1"};
    }
}