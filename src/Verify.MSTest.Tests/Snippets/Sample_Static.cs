namespace TheTests;

#region SampleTestMSTest_Static

[TestClass]
public class Sample_Static
{
    [TestMethod]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}

#endregion