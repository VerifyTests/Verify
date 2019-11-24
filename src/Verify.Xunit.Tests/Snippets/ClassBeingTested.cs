using System;
using System.Collections.Generic;

#region ClassBeingTested
public static class ClassBeingTested
{
    public static Person FindPerson()
    {
        return new Person
        {
            Id = new Guid("ebced679-45d3-4653-8791-3d969c4a986c"),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new List<string>
            {
                "Sam",
                "Mary"
            },
            Address = new Address
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };
    }
}
#endregion