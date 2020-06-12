using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

#region SampleTestMSTest
[TestClass]
public class Sample
{
    [TestMethod]
    public Task Test ()
    {
        var person = ClassBeingTested.FindPerson();
        return Verifier.Verify(person);
    }
}
#endregion