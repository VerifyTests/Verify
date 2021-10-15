using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VerifyTests;
using VerifyXunit;
using Xunit;

#region SortProperties
static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.SortPropertiesAlphabetically();
    }
}
#endregion

[UsesVerify]
public class SortedPropertiesTests
{
    #region SortPropertiesUsage
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

        return Verifier.Verify(person);
    }
    #endregion

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

        return Verifier.Verify(person)
            .AddExtraSettings(
                _ => { _.TypeNameHandling = TypeNameHandling.All; });
    }

    [Fact]
    public Task SymbolOrdering()
    {
        var target = new Dictionary<string, int>
        {
            {"#", 1},
            {"@", 1},
        };

        return Verifier.Verify(target);
    }

    [Fact]
    public Task JObject()
    {
        var obj = new JObject(
            new JProperty("@xmlns", "foo"),
            new JProperty("#text", "bar")
        );

        return Verifier.Verify(obj);
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
    Mr,
    Mrs
}