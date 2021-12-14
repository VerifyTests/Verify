using Newtonsoft.Json;

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

        return Verify(person);
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

        return Verify(person)
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
    Mr,
    Mrs
}