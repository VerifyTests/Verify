using System.Collections.Generic;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

#region xunitComplexMemberData
[UsesVerify]
public class ComplexParametersSample
{
    static ComplexParametersSample()
    {
        //TODO: this should be in the appdomain startup code
        VerifierSettings.NameForParameter<ComplexData>(_ => _.Value);
    }

    [Theory]
    [MemberData(nameof(GetComplexMemberData))]
    public Task ComplexMemberData(ComplexData arg)
    {
        VerifySettings settings = new();
        settings.UseParameters(arg);
        return Verifier.Verify(arg, settings);
    }

    [Theory]
    [MemberData(nameof(GetComplexMemberData))]
    public Task ComplexMemberDataFluent(ComplexData arg)
    {
        return Verifier.Verify(arg)
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