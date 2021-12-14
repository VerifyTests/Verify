#region xunitComplexMemberData
[UsesVerify]
public class ComplexParametersSample
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.NameForParameter<ComplexData>(_ => _.Value);
    }

    [Theory]
    [MemberData(nameof(GetComplexMemberData))]
    public Task ComplexMemberData(ComplexData arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetComplexMemberData))]
    public Task ComplexMemberDataFluent(ComplexData arg)
    {
        return Verify(arg)
            .UseParameters(arg);
    }

    public static IEnumerable<object[]> GetComplexMemberData()
    {
        yield return new object[]
        {
            new ComplexData {Value = "Value1"}
        };
        yield return new object[]
        {
            new ComplexData {Value = "Value2"}
        };
    }

    public class ComplexData
    {
        public string Value { get; set; } = null!;
    }
}
#endregion