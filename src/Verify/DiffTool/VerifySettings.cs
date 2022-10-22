namespace VerifyTests;

public partial class VerifySettings
{
#if DiffEngine
    public bool IsDiffEnabled => diffEnabled;
    internal bool diffEnabled = !DiffRunner.Disabled;

    /// <summary>
    /// Disable using a diff tool for this test
    /// </summary>
    public void DisableDiff() =>
        diffEnabled = false;
#else
    /// <summary>
    /// Disable using a diff tool for this test
    /// </summary>
    public void DisableDiff()
    {
    }
#endif
}