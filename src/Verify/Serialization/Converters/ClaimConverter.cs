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

        writer.WritePropertyName(claim.Type);
        writer.WriteRawValue(claim.Value);

        if (claim.Properties.Any())
        {
            writer.WritePropertyName("Claims");
            serializer.Serialize(writer, claim.Properties);
        }

        if (claim.Subject != null)
        {
            writer.WritePropertyName("Subject");
            serializer.Serialize(writer, claim.Subject);
        }

        writer.WriteEndObject();
    }
}