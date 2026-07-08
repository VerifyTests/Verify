public class ScrubMemberGlobalTests :
    BaseTest
{
    // The static ScrubMember(s) overloads previously delegated to Ignore*, so a
    // member was silently removed instead of rendered as "Scrubbed".

    [Fact]
    public Task ScrubMemberGeneric()
    {
        VerifierSettings.ScrubMember<Target>("Value");
        return Verify(new Target());
    }

    [Fact]
    public Task ScrubMembersGeneric()
    {
        VerifierSettings.ScrubMembers<Target>("Value");
        return Verify(new Target());
    }

    [Fact]
    public Task ScrubMembersType()
    {
        VerifierSettings.ScrubMembers(typeof(Target), "Value");
        return Verify(new Target());
    }

    public class Target
    {
        public string Value { get; set; } = "secret";
        public string Other { get; set; } = "keep";
    }
}
