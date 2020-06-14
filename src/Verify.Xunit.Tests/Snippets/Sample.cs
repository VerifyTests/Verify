using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

#region SampleTestXunit
[UsesVerify]
public class Sample
{
    [Fact]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verifier.Verify(person);
    }
}
#endregion