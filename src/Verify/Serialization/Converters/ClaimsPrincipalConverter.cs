using System.Security.Claims;

class ClaimsPrincipalConverter :
    WriteOnlyJsonConverter<ClaimsPrincipal>
{
    public override void Write(VerifyJsonWriter writer, ClaimsPrincipal principal)
    {
        // The object is always written, even with no identities. Writing no token at all
        // leaves the writer in Property state after the caller has written the member
        // name, which corrupts every subsequent write. WriteMember drops the empty
        // Identities collection, so an empty principal renders as {}.
        writer.WriteStartObject();
        writer.WriteMember(principal, principal.Identities, "Identities");
        writer.WriteEndObject();
    }
}