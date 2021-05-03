using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;
using VerifyTests;

class ClaimsIdentityConverter :
    WriteOnlyJsonConverter<ClaimsIdentity>
{
    public override void WriteJson(
        JsonWriter writer,
        ClaimsIdentity identity,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("Claims");
        serializer.Serialize(writer, identity.Claims);

        if (identity.Actor != null)
        {
            writer.WritePropertyName("Actor");
            serializer.Serialize(writer, identity.Actor);
        }

        if (identity.Actor != null)
        {
            writer.WritePropertyName("AuthenticationType");
            writer.WriteRawValue(identity.AuthenticationType);
        }

        if (identity.Label != null)
        {
            writer.WritePropertyName("Label");
            writer.WriteRawValue(identity.Label);
        }

        if (identity.NameClaimType != ClaimTypes.Name)
        {
            writer.WritePropertyName("NameClaimType");
            writer.WriteRawValue(identity.NameClaimType);
        }

        if (identity.RoleClaimType != ClaimTypes.Role)
        {
            writer.WritePropertyName("RoleClaimType");
            writer.WriteRawValue(identity.RoleClaimType);
        }

        writer.WriteEndObject();
    }
}