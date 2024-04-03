public class DisableScrubUserProfileTests :
    BaseTest
{
    string profileDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public DisableScrubUserProfileTests() =>
        VerifierSettings.DontScrubUserProfile();

    [Fact(Skip = "explicit")]
    public Task Simple() =>
        Verify(profileDir);

    [Fact(Skip = "explicit")]
    public Task Wrapped() =>
        Verify($" {profileDir} ");
}