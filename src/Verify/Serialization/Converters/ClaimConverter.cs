using System.Security.Claims;
using Newtonsoft.Json;
using VerifyTests;

class ClaimConverter :
    WriteOnlyJsonConverter<Claim>
{
    public override void Write(
        VerifyJsonWriter writer,
        Claim claim,
        JsonSerializer serializer)
    {
        writer.WriteStartObject();

        var type = claim.Type
            .Replace("http://schemas.xmlsoap.org/ws/2009/09/identity/claims/", "")
            .Replace("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/", "")
            .Replace("http://schemas.microsoft.com/ws/2008/06/identity/claims/", "");
        writer.WritePropertyName(type);
        writer.WriteRawValue(claim.Value);

        writer.WriteProperty(claim, _ => _.Properties);

        if (claim.Subject is {Name: { }})
        {
            writer.WriteProperty(claim, _ => _.Subject);
        }

        writer.WriteEndObject();
    }
}