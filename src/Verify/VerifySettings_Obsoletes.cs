namespace VerifyTests;

public partial class VerifySettings
{
    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    [Obsolete("Use VerifySettings.AutoVerify(bool includeBuildServer, bool throwException)")]
    public void AutoVerify(bool includeBuildServer = true) =>
        AutoVerify(includeBuildServer, false);

    /// <summary>
    /// Automatically accept the results of the current test.
    /// </summary>
    [Obsolete("Use VerifySettings.AutoVerify(AutoVerify autoVerify, bool includeBuildServer, bool throwException)")]
    public void AutoVerify(AutoVerify autoVerify, bool includeBuildServer = true) =>
        AutoVerify(autoVerify, includeBuildServer, false);
}
