using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

#region SampleTestXunit
using static VerifyXunit.Verifier;

[UsesVerify]
public class Sample
{
    [Fact]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}
#endregion