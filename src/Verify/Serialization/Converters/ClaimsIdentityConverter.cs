using System.Security.Claims;

class ClaimsIdentityConverter :
    WriteOnlyJsonConverter<ClaimsIdentity>
{
    public override void Write(VerifyJsonWriter writer, ClaimsIdentity identity)
    {
        writer.WriteStartObject();

        writer.WriteMember(identity, identity.Claims, "Claims");
        writer.WriteMember(identity, identity.Actor, "Actor");
        writer.WriteMember(identity, identity.AuthenticationType, "AuthenticationType");
        writer.WriteMember(identity, identity.Label, "Label");

        if (identity.NameClaimType != ClaimTypes.Name)
        {
            writer.WriteMember(identity, identity.NameClaimType, "NameClaimType");
        }

        if (identity.RoleClaimType != ClaimTypes.Role)
        {
            writer.WriteMember(identity, identity.RoleClaimType, "RoleClaimType");
        }

        writer.WriteEndObject();
    }
}