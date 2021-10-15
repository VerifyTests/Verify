using Microsoft.VisualStudio.TestTools.UnitTesting;
using VerifyMSTest;

namespace TheTests;

#region SampleTestMSTest

[TestClass]
public class Sample :
    VerifyBase
{
    [TestMethod]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}

#endregion