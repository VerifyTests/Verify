using System.Threading.Tasks;
using VerifyXunit;

#region SampleTestXunit
public class Sample
{
    [VerifyFact]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verifier.Verify(person);
    }

}
#endregion