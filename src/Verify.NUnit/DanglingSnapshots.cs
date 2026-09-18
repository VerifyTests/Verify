namespace VerifyNUnit;

[Experimental("VerifyDanglingSnapshots")]
public static class DanglingSnapshots
{
    public static void Run()
    {
        try
        {
            DanglingSnapshotsCheck.Run();
        }
        catch (Exception exception)
        {
            FailProcessOnExit(exception);
            throw;
        }
    }

    /// <summary>
    /// NUnit's Microsoft.Testing.Platform runner prints a teardown failure but does not count it: the
    /// run is summarised as passed and the process exits zero, so a dangling snapshot would not fail
    /// a build. That holds for <c>[SetUpFixture]</c> and <c>[TestFixture]</c> alike, and there is no
    /// teardown hook whose failure the runner does surface, so the exit code is set here instead.
    ///
    /// Set from ProcessExit, because the runner's own result replaces anything assigned while it is
    /// still running. Registered only once the check has already failed, and only over a zero, so a
    /// run that failed for its own reasons keeps the code it earned.
    /// </summary>
    static void FailProcessOnExit(Exception exception) =>
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            if (Environment.ExitCode != 0)
            {
                return;
            }

            Environment.ExitCode = 1;
            // Written from the handler so it lands after the runner's summary, which is where a
            // passing summary sitting next to a failing exit code needs explaining.
            Console.Error.WriteLine(
                $"""
                 Verify has failed the dangling snapshot check. The NUnit runner does not count a teardown failure, so it reported the run as passed; the exit code has been set to 1.
                 {exception.Message}
                 """);
        };
}
