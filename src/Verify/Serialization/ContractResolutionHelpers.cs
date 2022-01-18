using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VerifyTests;

public static class ContractResolutionHelpers
{
    public static void ConfigureIfBool(this JsonProperty property, MemberInfo member, bool dontIgnoreFalse)
    {
        if (dontIgnoreFalse)
        {
            if (property.PropertyType == typeof(bool))
            {
                property.DefaultValueHandling = DefaultValueHandling.Include;
            }
        }
    }
}