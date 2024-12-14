#region NameForParametersXunitV3

public class ComplexParametersSample
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.NameForParameter<ComplexData>(_ => _.Value);
        VerifierSettings.NameForParameter<ComplexStructData>(_ => _.Value);
    }

    [Theory]
    [MemberData(nameof(GetData))]
    public Task ComplexMemberData(ComplexData arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetData))]
    public Task ComplexMemberDataFluent(ComplexData arg) =>
        Verify(arg)
            .UseParameters(arg);

    [Theory]
    [MemberData(nameof(GetData))]
    public Task ComplexMemberNullableData(ComplexData arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetData))]
    public Task ComplexMemberNullableDataFluent(ComplexData arg) =>
        Verify(arg)
            .UseParameters(arg);

    public static IEnumerable<object[]> GetData()
    {
        yield return
        [
            new ComplexData("Value1")
        ];
        yield return
        [
            new ComplexData("Value2")
        ];
    }

    public record ComplexData(string Value);

    [Theory]
    [MemberData(nameof(GetStructData))]
    public Task ComplexMemberStructData(ComplexStructData arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetStructData))]
    public Task ComplexMemberStructDataFluent(ComplexStructData arg) =>
        Verify(arg)
            .UseParameters(arg);

    [Theory]
    [MemberData(nameof(GetStructData))]
    public Task ComplexMemberNullableStructData(ComplexStructData arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetStructData))]
    public Task ComplexMemberNullableStructDataFluent(ComplexStructData arg) =>
        Verify(arg)
            .UseParameters(arg);

    public static IEnumerable<object[]> GetStructData()
    {
        yield return [new ComplexStructData("Value1")];
        yield return [new ComplexStructData("Value2")];
    }

    public record ComplexStructData(string Value);
}

#endregion