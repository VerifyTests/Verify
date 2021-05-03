using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using VerifyTests;

class ClaimConverter :
    WriteOnlyJsonConverter<Claim>
{
    public override void WriteJson(
        JsonWriter writer,
        Claim claim,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();

        var type = claim.Type
            .Replace("http://schemas.xmlsoap.org/ws/2009/09/identity/claims/", "")
            .Replace("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/", "")
            .Replace("http://schemas.microsoft.com/ws/2008/06/identity/claims/", "");
        writer.WritePropertyName(type);
        writer.WriteRawValue(claim.Value);

        if (claim.Properties.Any())
        {
            writer.WritePropertyName("Properties");
            serializer.Serialize(writer, claim.Properties);
        }

        if (claim.Subject is {Name: { }})
        {
            writer.WritePropertyName("Subject");
            serializer.Serialize(writer, claim.Subject);
        }

        writer.WriteEndObject();
    }
}