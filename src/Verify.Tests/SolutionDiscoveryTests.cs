public class SolutionDiscoveryTests
{
    [Fact]
    public async Task SingleSlnxFile()
    {
        var testName = nameof(SingleSlnxFile);
        using var directory = new TempDirectory();
        var tempDir = directory.Path;

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

    [Fact]
    public async Task SingleSlnFile()
    {
        var testName = nameof(SingleSlnFile);
        using var directory = new TempDirectory();
        var tempDir = directory.Path;

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

    [Fact]
    public async Task BothSlnxAndSln_PreferSlnx()
    {
        var testName = nameof(BothSlnxAndSln_PreferSlnx);
        using var directory = new TempDirectory();
        var tempDir = directory.Path;

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

    [Fact]
    public async Task MultipleSlnxFiles_ShowsWarning()
    {
        var testName = nameof(MultipleSlnxFiles_ShowsWarning);
        using var directory = new TempDirectory();
        var tempDir = directory.Path;

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

    [Fact]
    public async Task MultipleSlnFiles_ShowsWarning()
    {
        var testName = nameof(MultipleSlnFiles_ShowsWarning);
        using var directory = new TempDirectory();
        var tempDir = directory.Path;

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

    [Fact]
    public async Task SolutionInParentDirectory()
    {
        var testName = nameof(SolutionInParentDirectory);
        using var directory = new TempDirectory();
        var tempDir = directory.Path;

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

    [Fact]
    public async Task SolutionInParentsParentDirectory()
    {
        var testName = nameof(SolutionInParentsParentDirectory);
        using var directory = new TempDirectory();
        var tempDir = directory.Path;

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

    [Fact]
    public async Task NoSolutionFile_NoMetadata()
    {
        var testName = nameof(NoSolutionFile_NoMetadata);
        using var directory = new TempDirectory();
        var tempDir = directory.Path;

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

    private static string CreateMinimalCsprojContent(string projectName)
    {
        // Get the path to Verify.csproj and Verify.props relative to test project
        var verifyProjectPath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(SolutionDiscoveryTests).Assembly.Location)!,
            "..", "..", "..", "..", "Verify", "Verify.csproj"));

        var verifyPropsPath = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(SolutionDiscoveryTests).Assembly.Location)!,
            "..", "..", "..", "..", "Verify", "buildTransitive", "Verify.props"));

        return $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>net10.0</TargetFramework>
                    <OutputType>Library</OutputType>
                    <AssemblyName>{projectName}</AssemblyName>
                  </PropertyGroup>
                  <ItemGroup>
                    <ProjectReference Include="{verifyProjectPath}" />
                  </ItemGroup>
                  <Import Project="{verifyPropsPath}" />
                </Project>
                """;
    }

    static string CreateMinimalSlnxContent() =>
        """
        {
          "solution": {
            "path": "TestSolution.slnx",
            "version": "1.0"
          }
        }
        """;

    static string CreateMinimalSlnContent() =>
        """

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

        """;

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
        // Use MetadataReader to read assembly attributes without loading the assembly
        using var fileStream = File.OpenRead(assemblyPath);
        using var peReader = new PEReader(fileStream);
        var metadataReader = peReader.GetMetadataReader();

        string? solutionDir = null;
        string? solutionName = null;

        foreach (var attributeHandle in metadataReader.GetAssemblyDefinition().GetCustomAttributes())
        {
            var attribute = metadataReader.GetCustomAttribute(attributeHandle);

            // Check if this is AssemblyMetadataAttribute
            if (attribute.Constructor.Kind == HandleKind.MemberReference)
            {
                var constructor = metadataReader.GetMemberReference((MemberReferenceHandle)attribute.Constructor);
                var attributeType = constructor.Parent;

                if (attributeType.Kind == HandleKind.TypeReference)
                {
                    var typeRef = metadataReader.GetTypeReference((TypeReferenceHandle)attributeType);
                    var typeName = metadataReader.GetString(typeRef.Name);
                    var typeNamespace = metadataReader.GetString(typeRef.Namespace);

                    if (typeName == "AssemblyMetadataAttribute" && typeNamespace == "System.Reflection")
                    {
                        var value = attribute.DecodeValue(new CustomAttributeTypeProvider());
                        if (value.FixedArguments.Length == 2)
                        {
                            var key = value.FixedArguments[0].Value as string;
                            var val = value.FixedArguments[1].Value as string;

                            if (key == "Verify.SolutionDirectory")
                            {
                                solutionDir = val;
                            }
                            else if (key == "Verify.SolutionName")
                            {
                                solutionName = val;
                            }
                        }
                    }
                }
            }
        }

        return (solutionDir, solutionName);
    }

    private class CustomAttributeTypeProvider : ICustomAttributeTypeProvider<object>
    {
        public object GetPrimitiveType(PrimitiveTypeCode typeCode) => typeCode;
        public object GetSystemType() => typeof(Type);
        public object GetSZArrayType(object elementType) => elementType;
        public object GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) => reader.GetTypeDefinition(handle);
        public object GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind) => reader.GetTypeReference(handle);
        public object GetTypeFromSerializedName(string name) => name;
        public PrimitiveTypeCode GetUnderlyingEnumType(object type) => PrimitiveTypeCode.Int32;
        public bool IsSystemType(object type) => type is Type;
    }
}
