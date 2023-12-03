#region SampleTestFixie

public class Sample
{
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}

#endregion