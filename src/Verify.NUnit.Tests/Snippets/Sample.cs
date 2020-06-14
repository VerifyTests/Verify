using System.Threading.Tasks;
using NUnit.Framework;

#region SampleTestNUnit
using static VerifyNUnit.Verifier;

[TestFixture]
public class Sample
{
    [Test]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}
#endregion