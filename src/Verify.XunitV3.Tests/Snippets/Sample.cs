﻿#region SampleTestXunitV3

public class Sample
{
    [Fact]
    public Task Test()
    {
        var person = ClassBeingTested.FindPerson();
        return Verify(person);
    }
}

#endregion