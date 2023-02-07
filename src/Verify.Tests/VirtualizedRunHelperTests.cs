public class VirtualizedRunHelperTests
{
    [Fact]
    public void Test_built_on_windows_run_on_linux_wsl()
    {
        var helper = new VirtualizedRunHelper(@"C:\build\project_dir", "/mnt/c/build/project_dir/subdir", '/', s => s switch
        {
            "/mnt/c/build/project_dir" => true,
            "/mnt/c/build/project_dir/src/file.cs" => true,
            _ => false
        });

        Assert.True(helper.AppearsToBeLocalVirtualizedRun);

        Assert.Equal("/mnt/c/build/project_dir/src/file.cs", helper.GetMappedBuildPath(@"C:\build\project_dir\src\file.cs"));
    }


    [Fact]
    public void Test_built_on_windows_run_on_linux_ci()
    {
        var helper = new VirtualizedRunHelper(@"C:\build\project_dir", "/ci/build/project_dir/subdir", '/', s => s switch
        {
            "/ci/build/project_dir" => true,
            "/ci/build/project_dir/src/file.cs" => true,
            _ => false
        });

        Assert.True(helper.AppearsToBeLocalVirtualizedRun);

        Assert.Equal("/ci/build/project_dir/src/file.cs", helper.GetMappedBuildPath(@"C:\build\project_dir\src\file.cs"));
    }
}