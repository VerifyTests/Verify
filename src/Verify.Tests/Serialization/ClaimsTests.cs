public class ClaimsTests
{
    [Fact]
    public Task EmptyClaimsPrincipal() =>
        // An empty principal still writes an object. Writing no token would leave the
        // writer mid-property and corrupt every member written after it.
        Verify(
            new
            {
                Principal = new ClaimsPrincipal(),
                Name = "TheValue"
            });

    [Fact]
    public Task ClaimsPrincipalWithIdentity() =>
        Verify(
            new
            {
                Principal = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        [new Claim("TheClaimType", "TheClaimValue")],
                        "TheAuthenticationType")),
                Name = "TheValue"
            });
}
