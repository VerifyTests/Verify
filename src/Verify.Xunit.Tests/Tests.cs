[UsesVerify]
public class Tests
{
    [Theory]
    [InlineData("Value1")]
    public Task MissingParameter(string arg) =>
        Verify("Foo");

    [Theory]
    [MemberData(nameof(GetData))]
    public Task UseFileNameWithParam(string arg) =>
        Verify(arg)
            .UseFileName("UseFileNameWithParam");


    public static IEnumerable<object[]> GetData() =>
        new[]
        {
            new object[]
            {
                "Value1"
            }
        };

    #region ExplicitTargetsXunit

    [Fact]
    public Task WithTargets() =>
        Verify(
            new
            {
                Property = "Value"
            },
            new[]
            {
                new Target(
                    extension: "txt",
                    stringData: "Raw target value",
                    name: "targetName")
            });

    #endregion
}