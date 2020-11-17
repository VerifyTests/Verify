using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VerifyTests;
using VerifyXunit;
using Xunit;

// Non-nullable field is uninitialized
#pragma warning disable CS8618

[UsesVerify]
public class VerifyObjectSamples
{
    async Task ChangeDefaultsPerVerification(object target)
    {
        #region ChangeDefaultsPerVerificationXunit
        var settings = new VerifySettings();
        settings.ModifySerialization(_ =>
        {
            _.DontIgnoreEmptyCollections();
            _.DontScrubGuids();
            _.DontScrubDateTimes();
            _.DontIgnoreFalse();
        });
        await Verifier.Verify(target, settings);
        #endregion
    }

    [Fact]
    public async Task ScopedSerializer()
    {
        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith",
        };
        var settings = new VerifySettings();
        settings.AddExtraSettings(_ => _.TypeNameHandling = TypeNameHandling.All);
        await Verifier.Verify(person, settings);
    }

    async Task Before()
    {
        #region Before

        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Address = new Address
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        await Verifier.Verify(person);

        #endregion
    }

    #region AnonXunit
    [Fact]
    public async Task Anon()
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

        await Verifier.Verify(
            new
            {
                person1,
                person2
            });
    }
    #endregion

    async Task After()
    {
        #region After

        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Address = new Address
            {
                Street = "1 Puddle Lane",
                Suburb = "Gotham",
                Country = "USA"
            }
        };

        await Verifier.Verify(person);

        #endregion
    }

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