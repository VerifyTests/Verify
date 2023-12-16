#region xunitComplexMemberData

[UsesVerify]
public class ComplexParametersSample
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.NameForParameter<ComplexData>(_ => _.Value);
        VerifierSettings.NameForParameter<ComplexStructData>(_ => _.Value);
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
    public Task ComplexMemberDataFluent(ComplexData arg) =>
        Verify(arg)
            .UseParameters(arg);

    [Theory]
    [MemberData(nameof(GetComplexMemberData))]
    public Task ComplexMemberNullableData(ComplexData arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetComplexMemberData))]
    public Task ComplexMemberNullableDataFluent(ComplexData arg) =>
        Verify(arg)
            .UseParameters(arg);

    public static IEnumerable<object[]> GetComplexMemberData()
    {
        yield return
        [
            new ComplexData
            {
                Value = "Value1"
            }
        ];
        yield return
        [
            new ComplexData
            {
                Value = "Value2"
            }
        ];
    }

    public class ComplexData
    {
        public string Value { get; set; } = null!;
    }

    [Theory]
    [MemberData(nameof(GetComplexMemberStructData))]
    public Task ComplexMemberStructData(ComplexStructData arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetComplexMemberStructData))]
    public Task ComplexMemberStructDataFluent(ComplexStructData arg) =>
        Verify(arg)
            .UseParameters(arg);

    [Theory]
    [MemberData(nameof(GetComplexMemberStructData))]
    public Task ComplexMemberNullableStructData(ComplexStructData arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetComplexMemberStructData))]
    public Task ComplexMemberNullableStructDataFluent(ComplexStructData arg) =>
        Verify(arg)
            .UseParameters(arg);

    public static IEnumerable<object[]> GetComplexMemberStructData()
    {
        yield return
        [
            new ComplexStructData("Value1")
        ];
        yield return
        [
            new ComplexStructData("Value2")
        ];
    }

    public struct ComplexStructData
    {
        public ComplexStructData(string value) =>
            Value = value;

        public string Value { get; set; } = null!;
    }
}

#endregion