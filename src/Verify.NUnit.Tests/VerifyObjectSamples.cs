using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using VerifyTests;
using VerifyNUnit;

// Non-nullable field is uninitialized
#pragma warning disable CS8618

[TestFixture]
public class VerifyObjectSamples
{
    async Task ChangeDefaultsPerVerification(object target)
    {
        #region ChangeDefaultsPerVerification

        await Verifier.Verify(target)
            .ModifySerialization(_ =>
            {
                _.DontIgnoreEmptyCollections();
                _.DontScrubGuids();
                _.DontScrubDateTimes();
                _.DontIgnoreFalse();
            });

        #endregion
    }

    [Test]
    public async Task ScopedSerializer()
    {
        Person person = new()
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Dob = new(2000, 10, 1, 0, 0, 0, TimeSpan.Zero),
        };

        VerifySettings settings = new();
        settings.ModifySerialization(_ => _.DontScrubDateTimes());
        settings.AddExtraSettings(_ => _.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat);
        await Verifier.Verify(person, settings);
    }

    [Test]
    public async Task ScopedSerializerFluent()
    {
        Person person = new()
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Dob = new(2000, 10, 1, 0, 0, 0, TimeSpan.Zero),
        };

        await Verifier.Verify(person)
            .ModifySerialization(_ => _.DontScrubDateTimes())
            .AddExtraSettings(_ => _.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat);
    }

    async Task Before()
    {
        #region Before

        Person person = new()
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Address = new()
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        await Verifier.Verify(person);

        #endregion
    }

    #region AnonNUnit

    [Test]
    public async Task Anon()
    {
        Person person1 = new()
        {
            GivenNames = "John",
            FamilyName = "Smith"
        };
        Person person2 = new()
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

        Person person = new()
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Address = new()
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