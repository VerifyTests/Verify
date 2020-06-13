using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

#region SampleTestXunit
[InjectInfo]
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