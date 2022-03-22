using System.Security.Claims;

class ClaimsPrincipalConverter :
    WriteOnlyJsonConverter<ClaimsPrincipal>
{
    public override void Write(VerifyJsonWriter writer, ClaimsPrincipal principal)
    {
        if (!principal.Identities.Any())
        {
            return;
        }

        writer.WriteStartObject();
        writer.WriteProperty(principal, principal.Identities, "Identities");
        writer.WriteEndObject();
    }
}