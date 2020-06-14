using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

#region SampleTestMSTest
[TestClass]
public class Sample :
    VerifyBase
{
    [TestMethod]
    public Task Test ()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}
#endregion