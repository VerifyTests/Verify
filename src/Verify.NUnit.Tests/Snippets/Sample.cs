﻿#region SampleTestNUnit

[TestFixture]
public class Sample
{
    [Test]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verifier.Verify(person);
    }
}
#endregion