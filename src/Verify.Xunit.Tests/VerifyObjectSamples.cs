using Argon;

// ReSharper disable NotAccessedField.Local

// Non-nullable field is uninitialized
#pragma warning disable CS8618

[UsesVerify]
public class VerifyObjectSamples
{
    [Fact]
    public Task ScopedSerializer()
    {
        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith"
        };
        var settings = new VerifySettings();
        settings.AddExtraSettings(_ => _.TypeNameHandling = TypeNameHandling.All);
        return Verify(person, settings);
    }

    #region AnonXunit

    [Fact]
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