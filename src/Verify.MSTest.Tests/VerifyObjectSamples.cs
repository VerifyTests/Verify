using Argon;

// ReSharper disable NotAccessedField.Local

namespace TheTests;
// Non-nullable field is uninitialized
#pragma warning disable CS8618

[TestClass]
public class VerifyObjectSamples :
    VerifyBase
{
    [TestMethod]
    public Task ScopedSerializer()
    {
        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Dob = new(2000, 10, 1, 0, 0, 0, TimeSpan.Zero)
        };
        var settings = new VerifySettings();
        settings.AddExtraSettings(_ => _.TypeNameHandling = TypeNameHandling.All);
        return Verify(person, settings);
    }

    #region AnonMSTest

    [TestMethod]
    public Task Anon()
    {
        var person1 = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith"
        };
        var person2 = new Person
        {
            GivenNames = "Marianne",
            FamilyName = "Aguirre"
        };

        return Verify(
            new
            {
                person1,
                person2
            });
    }

    #endregion

    class Person
    {
        public string GivenNames;
        public string FamilyName;
        public string Spouse;
        public Address Address;
        public DateTimeOffset Dob;
    }

    class Address
    {
        public string Street;
        public string Country;
        public string Suburb;
    }
}