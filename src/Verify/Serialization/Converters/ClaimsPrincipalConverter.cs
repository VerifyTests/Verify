using System.Security.Claims;
using Newtonsoft.Json;
using VerifyTests;

class ClaimsPrincipalConverter :
    WriteOnlyJsonConverter<ClaimsPrincipal>
{
    public override void WriteJson(
        VerifyJsonTextWriter writer,
        ClaimsPrincipal principal,
        JsonSerializer serializer)
    {
        serializer.Serialize(writer, principal.Identities);
    }
}