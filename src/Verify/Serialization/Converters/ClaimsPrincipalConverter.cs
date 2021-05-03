using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;
using VerifyTests;

class ClaimsPrincipalConverter :
    WriteOnlyJsonConverter<ClaimsPrincipal>
{
    public override void WriteJson(
        JsonWriter writer,
        ClaimsPrincipal principal,
        JsonSerializer serializer,
        IReadOnlyDictionary<string, object> context)
    {
        serializer.Serialize(writer, principal.Identities);
    }
}