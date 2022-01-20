using System.Security.Claims;
using Newtonsoft.Json;
using VerifyTests;

class ClaimsIdentityConverter :
    WriteOnlyJsonConverter<ClaimsIdentity>
{
    public override void Write(
        VerifyJsonWriter writer,
        ClaimsIdentity identity,
        JsonSerializer serializer)
    {
        writer.WriteStartObject();

        writer.WriteProperty(identity, _ => _.Claims);
        writer.WriteProperty(identity, _ => _.Actor);
        writer.WriteProperty(identity, _ => _.AuthenticationType);
        writer.WriteProperty(identity, _ => _.Label);

        if (identity.NameClaimType != ClaimTypes.Name)
        {
            writer.WriteProperty(identity, _ => _.NameClaimType);
        }

        if (identity.RoleClaimType != ClaimTypes.Role)
        {
            writer.WriteProperty(identity, _ => _.RoleClaimType);
        }

        writer.WriteEndObject();
    }
}