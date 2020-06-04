using System.Collections.Generic;
using System.Threading.Tasks;
using Verify;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

#region xunitComplexMemberData
public class ComplexParametersSample :
    VerifyBase
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
        // required since
        UseParameters(arg);
        return Verify(arg);
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

    public ComplexParametersSample(ITestOutputHelper output) :
        base(output)
    {
    }
}
#endregion