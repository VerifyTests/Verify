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

    public static void Machine(StringBuilder builder) =>
        PerformReplacements(builder, machineName, "TheMachineName");

    public static void User(StringBuilder builder) =>
        PerformReplacements(builder, userName, "TheUserName");
}