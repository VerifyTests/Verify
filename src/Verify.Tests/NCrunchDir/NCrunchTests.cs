#pragma warning disable CS8605
[UsesVerify]
public class NCrunchTests
{
    [Fact]
    public Task Simple()
    {
        foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
        {
            var key = (string) entry.Key;
            if (key.StartsWith("NCrunch", StringComparison.InvariantCulture) &&
                key != "NCrunch.AllAssemblyLocations")
            {
                Console.WriteLine($"{key} {entry.Value}");
            }
        }

        return Verify("Values");
    }
}