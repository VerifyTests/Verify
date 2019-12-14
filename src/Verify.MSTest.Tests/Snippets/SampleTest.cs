using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

#region SampleTestMSTest
[TestClass]
public class SampleTest :
    VerifyBase
{
    [TestMethod]
    public Task Simple()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}
#endregion