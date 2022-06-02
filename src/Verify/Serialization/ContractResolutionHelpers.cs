namespace VerifyTests;

public static class ContractResolutionHelpers
{
    public static void ConfigureIfBool(this JsonProperty property)
    {
        if (property.PropertyType == typeof(bool))
        {
            property.DefaultValueHandling = DefaultValueHandling.Include;
        }
    }
}