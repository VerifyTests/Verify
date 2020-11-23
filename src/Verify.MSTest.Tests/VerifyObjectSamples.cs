using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using VerifyTests;
using VerifyMSTest;

// Non-nullable field is uninitialized
#pragma warning disable CS8618

[TestClass]
public class VerifyObjectSamples :
    VerifyBase
{
    async Task ChangeDefaultsPerVerification(object target)
    {
        #region ChangeDefaultsPerVerification

        VerifySettings settings = new();

        settings.ModifySerialization(_ =>
        {
            _.DontIgnoreEmptyCollections();
            _.DontScrubGuids();
            _.DontScrubDateTimes();
            _.DontIgnoreFalse();
        });
        await Verify(target, settings);

        #endregion

        #region ChangeDefaultsPerVerification

        await Verify(target)
            .ModifySerialization(_ =>
            {
                _.DontIgnoreEmptyCollections();
                _.DontScrubGuids();
                _.DontScrubDateTimes();
                _.DontIgnoreFalse();
            });

        #endregion
    }

    [TestMethod]
    public async Task ScopedSerializer()
    {
        Person person = new()
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Dob = new(2000, 10, 1, 0, 0, 0, TimeSpan.Zero),
        };
        VerifySettings settings = new();
        settings.AddExtraSettings(_ => _.TypeNameHandling = TypeNameHandling.All);
        await Verify(person, settings);
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

        await Verify(person);

        #endregion
    }

    #region AnonMSTest

    [TestMethod]
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

        await Verify(
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

        await Verify(person);

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