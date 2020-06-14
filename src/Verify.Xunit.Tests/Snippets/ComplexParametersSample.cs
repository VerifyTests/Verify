using System.Collections.Generic;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;

#region xunitComplexMemberData
[UsesVerify]
public class ComplexParametersSample
{
    static ComplexParametersSample()
    {
        //TODO: this should be in the appdomain startup code
        SharedVerifySettings.NameForParameter<ComplexData>(_ => _.Value);
    }

    [Theory]
    [MemberData(nameof(GetComplexMemberData))]
    public Task ComplexMemberData(ComplexData arg)
    {
        var settings = new VerifySettings();
        settings.UseParameters(arg);
        return Verifier.Verify(arg, settings);
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