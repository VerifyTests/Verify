public class DeriveProjectRelativeDirectoryTests
{
    static readonly string projectDirectory = Path.Combine(Path.GetTempPath(), "project");

    static string? Derive(string sourceFile, bool mirrorSourceStructure) =>
        PathInfo.DeriveProjectRelative(
                "Snapshots",
                mirrorSourceStructure,
                sourceFile,
                projectDirectory,
                "TheType",
                "TheMethod")
            .Directory;

    [Fact]
    public void NotMirroredUsesRoot()
    {
        var source = Path.Combine(projectDirectory, "Foo", "MyTests.cs");
        Assert.Equal(Path.Combine(projectDirectory, "Snapshots"), Derive(source, false));
    }

    [Fact]
    public void MirrorsNestedSourceStructure()
    {
        var source = Path.Combine(projectDirectory, "Foo", "Bar", "MyTests.cs");
        Assert.Equal(Path.Combine(projectDirectory, "Snapshots", "Foo", "Bar"), Derive(source, true));
    }

    [Fact]
    public void MirrorAtProjectRootUsesRoot()
    {
        var source = Path.Combine(projectDirectory, "MyTests.cs");
        Assert.Equal(Path.Combine(projectDirectory, "Snapshots"), Derive(source, true));
    }

    [Fact]
    public void MirrorSourceOutsideProjectUsesRoot()
    {
        var source = Path.Combine(Path.GetTempPath(), "other", "MyTests.cs");
        Assert.Equal(Path.Combine(projectDirectory, "Snapshots"), Derive(source, true));
    }
}
