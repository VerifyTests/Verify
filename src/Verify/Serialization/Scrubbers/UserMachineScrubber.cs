static class UserMachineScrubber
{
    static string machineName;
    static string userName;

    static UserMachineScrubber() =>
        ResetReplacements(Environment.MachineName, Environment.UserName);

    [MemberNotNull(nameof(machineName), nameof(userName))]
    internal static void ResetReplacements(string machineName, string userName)
    {
        UserMachineScrubber.machineName = machineName;
        UserMachineScrubber.userName = userName;
    }

    public static Scrubber MachineScrubber() =>
        Scrubber.Replace(machineName, "TheMachineName", requireWordBoundary: true);

    public static Scrubber UserScrubber() =>
        Scrubber.Replace(userName, "TheUserName", requireWordBoundary: true);
}
