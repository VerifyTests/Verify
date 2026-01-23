using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using Xunit;

public class SolutionDiscoveryTests
{
    [Fact]
    public async Task SingleSlnxFile()
    {
        var testName = nameof(SingleSlnxFile);
        var tempDir = GetTempDirectory();
        try
        {
            // Create directory structure
            var projectDir = Path.Combine(tempDir, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Create .slnx file in same directory as project
            var slnxPath = Path.Combine(tempDir, "TestSolution.slnx");
            await File.WriteAllTextAsync(slnxPath, CreateMinimalSlnxContent());

            // Create .csproj file
            var csprojPath = Path.Combine(projectDir, $"{testName}.csproj");
            await File.WriteAllTextAsync(csprojPath, CreateMinimalCsprojContent(testName));

            // Build project
            var (success, output) = await BuildProject(csprojPath);
            Assert.True(success, $"Build failed: {output}");

            // Load assembly and verify metadata
            var assemblyPath = GetAssemblyPath(projectDir, testName);
            var (solutionDir, solutionName) = LoadAssemblyAndGetMetadata(assemblyPath);

            Assert.Equal(tempDir + Path.DirectorySeparatorChar, solutionDir);
            Assert.Equal("TestSolution", solutionName);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public async Task SingleSlnFile()
    {
        var testName = nameof(SingleSlnFile);
        var tempDir = GetTempDirectory();
        try
        {
            // Create directory structure
            var projectDir = Path.Combine(tempDir, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Create .sln file in same directory as project
            var slnPath = Path.Combine(tempDir, "TestSolution.sln");
            await File.WriteAllTextAsync(slnPath, CreateMinimalSlnContent());

            // Create .csproj file
            var csprojPath = Path.Combine(projectDir, $"{testName}.csproj");
            await File.WriteAllTextAsync(csprojPath, CreateMinimalCsprojContent(testName));

            // Build project
            var (success, output) = await BuildProject(csprojPath);
            Assert.True(success, $"Build failed: {output}");

            // Load assembly and verify metadata
            var assemblyPath = GetAssemblyPath(projectDir, testName);
            var (solutionDir, solutionName) = LoadAssemblyAndGetMetadata(assemblyPath);

            Assert.Equal(tempDir + Path.DirectorySeparatorChar, solutionDir);
            Assert.Equal("TestSolution", solutionName);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public async Task BothSlnxAndSln_PreferSlnx()
    {
        var testName = nameof(BothSlnxAndSln_PreferSlnx);
        var tempDir = GetTempDirectory();
        try
        {
            // Create directory structure
            var projectDir = Path.Combine(tempDir, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Create both .slnx and .sln files
            var slnxPath = Path.Combine(tempDir, "PreferredSolution.slnx");
            await File.WriteAllTextAsync(slnxPath, CreateMinimalSlnxContent());

            var slnPath = Path.Combine(tempDir, "OtherSolution.sln");
            await File.WriteAllTextAsync(slnPath, CreateMinimalSlnContent());

            // Create .csproj file
            var csprojPath = Path.Combine(projectDir, $"{testName}.csproj");
            await File.WriteAllTextAsync(csprojPath, CreateMinimalCsprojContent(testName));

            // Build project
            var (success, output) = await BuildProject(csprojPath);
            Assert.True(success, $"Build failed: {output}");

            // Load assembly and verify metadata - should use .slnx
            var assemblyPath = GetAssemblyPath(projectDir, testName);
            var (_, solutionName) = LoadAssemblyAndGetMetadata(assemblyPath);

            // Should prefer .slnx file
            Assert.Equal("PreferredSolution", solutionName);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public async Task MultipleSlnxFiles_ShowsWarning()
    {
        var testName = nameof(MultipleSlnxFiles_ShowsWarning);
        var tempDir = GetTempDirectory();
        try
        {
            // Create directory structure
            var projectDir = Path.Combine(tempDir, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Create multiple .slnx files
            await File.WriteAllTextAsync(Path.Combine(tempDir, "Solution1.slnx"), CreateMinimalSlnxContent());
            await File.WriteAllTextAsync(Path.Combine(tempDir, "Solution2.slnx"), CreateMinimalSlnxContent());

            // Create .csproj file
            var csprojPath = Path.Combine(projectDir, $"{testName}.csproj");
            await File.WriteAllTextAsync(csprojPath, CreateMinimalCsprojContent(testName));

            // Build project
            var (success, output) = await BuildProject(csprojPath);
            Assert.True(success, $"Build failed: {output}");

            // Should contain warning about multiple solution files
            Assert.Contains("Multiple solution files found", output);

            // Load assembly - should NOT have solution metadata
            var assemblyPath = GetAssemblyPath(projectDir, testName);
            var (solutionDir, solutionName) = LoadAssemblyAndGetMetadata(assemblyPath);

            Assert.Null(solutionDir);
            Assert.Null(solutionName);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public async Task MultipleSlnFiles_ShowsWarning()
    {
        var testName = nameof(MultipleSlnFiles_ShowsWarning);
        var tempDir = GetTempDirectory();
        try
        {
            // Create directory structure
            var projectDir = Path.Combine(tempDir, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Create multiple .sln files
            await File.WriteAllTextAsync(Path.Combine(tempDir, "Solution1.sln"), CreateMinimalSlnContent());
            await File.WriteAllTextAsync(Path.Combine(tempDir, "Solution2.sln"), CreateMinimalSlnContent());

            // Create .csproj file
            var csprojPath = Path.Combine(projectDir, $"{testName}.csproj");
            await File.WriteAllTextAsync(csprojPath, CreateMinimalCsprojContent(testName));

            // Build project
            var (success, output) = await BuildProject(csprojPath);
            Assert.True(success, $"Build failed: {output}");

            // Should contain warning about multiple solution files
            Assert.Contains("Multiple solution files found", output);

            // Load assembly - should NOT have solution metadata
            var assemblyPath = GetAssemblyPath(projectDir, testName);
            var (solutionDir, solutionName) = LoadAssemblyAndGetMetadata(assemblyPath);

            Assert.Null(solutionDir);
            Assert.Null(solutionName);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public async Task SolutionInParentDirectory()
    {
        var testName = nameof(SolutionInParentDirectory);
        var tempDir = GetTempDirectory();
        try
        {
            // Create directory structure: tempDir/src/TestProject
            var srcDir = Path.Combine(tempDir, "src");
            var projectDir = Path.Combine(srcDir, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Create .slnx file in parent directory
            var slnxPath = Path.Combine(srcDir, "TestSolution.slnx");
            await File.WriteAllTextAsync(slnxPath, CreateMinimalSlnxContent());

            // Create .csproj file
            var csprojPath = Path.Combine(projectDir, $"{testName}.csproj");
            await File.WriteAllTextAsync(csprojPath, CreateMinimalCsprojContent(testName));

            // Build project
            var (success, output) = await BuildProject(csprojPath);
            Assert.True(success, $"Build failed: {output}");

            // Load assembly and verify metadata
            var assemblyPath = GetAssemblyPath(projectDir, testName);
            var (solutionDir, solutionName) = LoadAssemblyAndGetMetadata(assemblyPath);

            Assert.Equal(srcDir + Path.DirectorySeparatorChar, solutionDir);
            Assert.Equal("TestSolution", solutionName);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public async Task SolutionInParentsParentDirectory()
    {
        var testName = nameof(SolutionInParentsParentDirectory);
        var tempDir = GetTempDirectory();
        try
        {
            // Create directory structure: tempDir/src/SubDir/TestProject
            var srcDir = Path.Combine(tempDir, "src");
            var subDir = Path.Combine(srcDir, "SubDir");
            var projectDir = Path.Combine(subDir, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Create .slnx file in parent's parent directory
            var slnxPath = Path.Combine(srcDir, "TestSolution.slnx");
            await File.WriteAllTextAsync(slnxPath, CreateMinimalSlnxContent());

            // Create .csproj file
            var csprojPath = Path.Combine(projectDir, $"{testName}.csproj");
            await File.WriteAllTextAsync(csprojPath, CreateMinimalCsprojContent(testName));

            // Build project
            var (success, output) = await BuildProject(csprojPath);
            Assert.True(success, $"Build failed: {output}");

            // Load assembly and verify metadata
            var assemblyPath = GetAssemblyPath(projectDir, testName);
            var (solutionDir, solutionName) = LoadAssemblyAndGetMetadata(assemblyPath);

            Assert.Equal(srcDir + Path.DirectorySeparatorChar, solutionDir);
            Assert.Equal("TestSolution", solutionName);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    [Fact]
    public async Task NoSolutionFile_NoMetadata()
    {
        var testName = nameof(NoSolutionFile_NoMetadata);
        var tempDir = GetTempDirectory();
        try
        {
            // Create directory structure
            var projectDir = Path.Combine(tempDir, "TestProject");
            Directory.CreateDirectory(projectDir);

            // Create .csproj file (no solution file)
            var csprojPath = Path.Combine(projectDir, $"{testName}.csproj");
            await File.WriteAllTextAsync(csprojPath, CreateMinimalCsprojContent(testName));

            // Build project
            var (success, output) = await BuildProject(csprojPath);
            Assert.True(success, $"Build failed: {output}");

            // Load assembly - should NOT have solution metadata
            var assemblyPath = GetAssemblyPath(projectDir, testName);
            var (solutionDir, solutionName) = LoadAssemblyAndGetMetadata(assemblyPath);

            Assert.Null(solutionDir);
            Assert.Null(solutionName);
        }
        finally
        {
            CleanupDirectory(tempDir);
        }
    }

    private static string GetTempDirectory()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "VerifyTests_" + Guid.NewGuid());
        Directory.CreateDirectory(tempPath);
        return tempPath;
    }

    private static string CreateMinimalCsprojContent(string projectName)
    {
        // Get the path to Verify.csproj relative to test project
        var verifyProjectPath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(SolutionDiscoveryTests).Assembly.Location)!,
            "..", "..", "..", "..", "Verify", "Verify.csproj"));

        return $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <OutputType>Library</OutputType>
    <AssemblyName>{projectName}</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""{verifyProjectPath}"" />
  </ItemGroup>
</Project>";
    }

    private static string CreateMinimalSlnxContent() =>
        @"{
  ""solution"": {
    ""path"": ""TestSolution.slnx"",
    ""version"": ""1.0""
  }
}";

    private static string CreateMinimalSlnContent() =>
        @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
EndGlobal
";

    private static async Task<(bool success, string output)> BuildProject(string csprojPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"build \"{csprojPath}\" --configuration Release --verbosity normal",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            return (false, "Failed to start process");
        }

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;
        var fullOutput = output + Environment.NewLine + error;

        return (process.ExitCode == 0, fullOutput);
    }

    private static string GetAssemblyPath(string projectDir, string assemblyName)
    {
        var binPath = Path.Combine(projectDir, "bin", "Release", "net10.0");
        var dllPath = Path.Combine(binPath, $"{assemblyName}.dll");

        if (!File.Exists(dllPath))
        {
            throw new FileNotFoundException($"Assembly not found at {dllPath}");
        }

        return dllPath;
    }

    private static (string? solutionDir, string? solutionName) LoadAssemblyAndGetMetadata(string assemblyPath)
    {
        // Create isolated AssemblyLoadContext to avoid conflicts
        var context = new AssemblyLoadContext(name: null, isCollectible: true);
        try
        {
            var assembly = context.LoadFromAssemblyPath(assemblyPath);
            var solutionDir = GetAssemblyMetadata(assembly, "Verify.SolutionDirectory");
            var solutionName = GetAssemblyMetadata(assembly, "Verify.SolutionName");
            return (solutionDir, solutionName);
        }
        finally
        {
            context.Unload();
        }
    }

    private static string? GetAssemblyMetadata(Assembly assembly, string key)
    {
        var attributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
        return attributes.FirstOrDefault(a => a.Key == key)?.Value;
    }

    private static void CleanupDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return;
        }

        try
        {
            // Give the system time to release file handles
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100);

            Directory.Delete(directory, true);
        }
        catch
        {
            // Ignore cleanup errors
        }
    }
}
