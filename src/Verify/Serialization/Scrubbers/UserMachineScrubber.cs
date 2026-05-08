static partial class UserMachineScrubber
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

    public static LiteralReplacePatternScrubber MachinePatternScrubber() =>
        new(machineName, "TheMachineName");

    public static LiteralReplacePatternScrubber UserPatternScrubber() =>
        new(userName, "TheUserName");
}