using System;
using System.Collections.Generic;

namespace Changed
{
    #region ClassBeingTestedChanged
    public static class ClassBeingTested
    {
        public static Person FindPerson()
        {
            return new Person
            {
                Id = new Guid("ebced679-45d3-4653-8791-3d969c4a986c"),
                Title = Title.Mr,
                // Middle name added
                GivenNames = "John James",
                FamilyName = "Smith",
                Spouse = "Jill",
                Children = new List<string>
                {
                    "Sam",
                    "Mary"
                },
                Address = new Address
                {
                    // Address changed
                    Street = "64 Barnett Street",
                    Country = "USA"
                }
            };
        }
    }
    #endregion
}