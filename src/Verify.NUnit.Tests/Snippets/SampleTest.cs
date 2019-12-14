using System.Threading.Tasks;
using NUnit.Framework;
using VerifyNUnit;

#region SampleTest
[TestFixture]
public class SampleTest
{
    [Test]
    public Task Simple()
    {
        var person = ClassBeingTested.FindPerson();
        return Verifier.Verify(person);
    }
}
#endregion