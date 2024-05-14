namespace TheTests;

#region SampleTestMSTest

[TestClass]
[UsesVerify]
public partial class Sample
{
    [TestMethod]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}

#endregion