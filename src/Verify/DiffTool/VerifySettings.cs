namespace VerifyTests;

public partial class VerifySettings
{
    internal bool diffEnabled = !DiffRunner.Disabled;

    /// <summary>
    /// Disable using a diff tool for this test
    /// </summary>
    public VerifySettings DisableDiff()
    {
        diffEnabled = false;
        return this;
    }
}