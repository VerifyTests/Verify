using System.Security.Claims;

class ClaimsIdentityConverter :
    WriteOnlyJsonConverter<ClaimsIdentity>
{
    public override void Write(VerifyJsonWriter writer, ClaimsIdentity identity)
    {
        writer.WriteStartObject();

        writer.WriteProperty(identity, identity.Claims, "Claims");
        writer.WriteProperty(identity, identity.Actor, "Actor");
        writer.WriteProperty(identity, identity.AuthenticationType, "AuthenticationType");
        writer.WriteProperty(identity, identity.Label, "Label");

        if (identity.NameClaimType != ClaimTypes.Name)
        {
            writer.WriteProperty(identity, identity.NameClaimType, "NameClaimType");
        }

        if (identity.RoleClaimType != ClaimTypes.Role)
        {
            writer.WriteProperty(identity, identity.RoleClaimType, "RoleClaimType");
        }

        writer.WriteEndObject();
    }
}