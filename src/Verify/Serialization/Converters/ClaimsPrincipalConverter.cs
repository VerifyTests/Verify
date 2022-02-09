using System.Security.Claims;

class ClaimsPrincipalConverter :
    WriteOnlyJsonConverter<ClaimsPrincipal>
{
    public override void Write(VerifyJsonWriter writer, ClaimsPrincipal principal)
    {
        writer.Serialize(principal.Identities);
    }
}