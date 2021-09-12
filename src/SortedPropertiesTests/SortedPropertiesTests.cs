using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VerifyTests;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class SortedPropertiesTests
{
    [Fact]
    public Task Alphabetically()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new() { "Sam", "Mary" },
            Address = new()
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        VerifierSettings.SortPropertiesAlphabetically();
        return Verifier.Verify(person);
    }

    [Fact]
    public Task DoesNotAffectTypeName()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Title = Title.Mr,
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new() { "Sam", "Mary" },
            Address = new()
            {
                Street = "1 Puddle Lane",
                Country = "USA"
            }
        };

        VerifierSettings.SortPropertiesAlphabetically();
        return Verifier.Verify(person)
            .AddExtraSettings(
                _ => { _.TypeNameHandling = TypeNameHandling.All; });
    }
}

#pragma warning disable CS8618
public class Person
{
    public string? GivenNames;
    public string FamilyName;
    public string Spouse;
    public Address Address;
    public List<string> Children;
    public Title Title;
    public Guid Id;
    public DateTimeOffset Dob;
}

public class Address
{
    public string Street;
    public string Suburb;
    public string Country;
}

public enum Title
{
    Mr, Mrs
}