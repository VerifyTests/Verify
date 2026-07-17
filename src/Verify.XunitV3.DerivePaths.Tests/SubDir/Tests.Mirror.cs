// This partial is intentionally located in a sub-directory so that its source file path,
// relative to the project directory, is "SubDir". That exercises the mirrorSourceStructure
// option, nesting the derived `.verified.` file under "Relative/SubDir".
public partial class Tests
{
    [Fact]
    public Task ProjectRelativeDirectoryMirrored()
    {
        VerifierSettings.Reset();
        UseProjectRelativeDirectory("Relative", mirrorSourceStructure: true);
        return Verify("Value");
    }
}
