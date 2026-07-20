#if NET10_0

public class AotCompatibilityTests
{
    [Fact]
    public async Task Serialize_AnonymousObject_UnderAot()
    {
        using var directory = new TempDirectory();
        var tempDir = directory.Path;

        var verifyProjectPath = Path.Combine(ProjectFiles.SolutionDirectory, "Verify", "Verify.csproj");
        var verifyPropsPath = Path.Combine(ProjectFiles.SolutionDirectory, "Verify", "buildTransitive", "Verify.props");

        var csprojContent = $"""
                             <Project Sdk="Microsoft.NET.Sdk">
                               <PropertyGroup>
                                 <OutputType>Exe</OutputType>
                                 <TargetFramework>net10.0</TargetFramework>
                                 <PublishAot>true</PublishAot>
                                 <ImplicitUsings>enable</ImplicitUsings>
                                 <InvariantGlobalization>true</InvariantGlobalization>
                               </PropertyGroup>
                               <ItemGroup>
                                 <ProjectReference Include="{verifyProjectPath}" />
                               </ItemGroup>
                               <Import Project="{verifyPropsPath}" />
                             </Project>
                             """;

        var programContent = """
                             using Argon;

                             try
                             {
                                 var settings = new JsonSerializerSettings
                                 {
                                     Formatting = Formatting.Indented,
                                     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                     DefaultValueHandling = DefaultValueHandling.Ignore,
                                 };
                                 var serializer = JsonSerializer.Create(settings);
                                 using var writer = new StringWriter();
                                 serializer.Serialize(writer, new { key = "value", number = 42 });
                                 Console.WriteLine("OK");
                                 return 0;
                             }
                             catch (Exception ex)
                             {
                                 Console.Error.WriteLine($"{ex.GetType().FullName}: {ex.Message}");
                                 return 1;
                             }
                             """;

        await File.WriteAllTextAsync(Path.Combine(tempDir, "AotTestApp.csproj"), csprojContent);
        await File.WriteAllTextAsync(Path.Combine(tempDir, "Program.cs"), programContent);

        var (publishSuccess, publishOutput) = await PublishProject(tempDir);
        if (!publishSuccess)
        {
            SkipIfAotToolchainMissing(publishOutput);
            Assert.Fail($"Publish failed:{Environment.NewLine}{ErrorLines(publishOutput)}");
        }

        var exePath = GetPublishedExePath(tempDir, "AotTestApp");
        var (exitCode, stdout, _) = await RunExecutable(exePath);

        Assert.Equal(0, exitCode);
        Assert.Contains("OK", stdout);
    }

    // A native AOT publish needs a platform linker, which on Windows comes from the "Desktop development
    // with C++" workload. That is a machine prerequisite rather than anything Verify controls, so name it
    // and skip, instead of reporting it as a Verify failure buried in a wall of publish output.
    static void SkipIfAotToolchainMissing(string publishOutput)
    {
        if (publishOutput.Contains("Platform linker not found"))
        {
            Assert.Skip("Native AOT prerequisites are missing: the platform linker was not found. On Windows install the 'Desktop development with C++' workload. See https://aka.ms/nativeaot-prerequisites");
        }
    }

    // The publish output is hundreds of lines of MSBuild noise, and the reason for the failure is the
    // handful of error lines in it.
    static string ErrorLines(string publishOutput)
    {
        var errors = publishOutput
            .Split('\n')
            .Select(_ => _.Trim())
            .Where(_ => _.Contains(": error ", StringComparison.Ordinal))
            .Distinct()
            .ToList();

        if (errors.Count == 0)
        {
            return publishOutput;
        }

        return string.Join(Environment.NewLine, errors);
    }

    static async Task<(bool success, string output)> PublishProject(string projectDir)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "publish -c Release",
            WorkingDirectory = projectDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Ensure vswhere.exe is discoverable for native AOT linking
        var vsInstallerPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            "Microsoft Visual Studio", "Installer");
        if (Directory.Exists(vsInstallerPath))
        {
            startInfo.Environment["PATH"] = vsInstallerPath + Path.PathSeparator + startInfo.Environment["PATH"];
        }

        using var process = Process.Start(startInfo)!;

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;
        var fullOutput = output + Environment.NewLine + error;

        return (process.ExitCode == 0, fullOutput);
    }

    static string GetPublishedExePath(string projectDir, string appName)
    {
        var exeName = OperatingSystem.IsWindows() ? $"{appName}.exe" : appName;
        var binDir = Path.Combine(projectDir, "bin", "Release");

        // Search recursively - AOT publish may place the exe in a RID-specific subdirectory
        foreach (var file in Directory.EnumerateFiles(binDir, exeName, SearchOption.AllDirectories))
        {
            return file;
        }

        throw new FileNotFoundException($"Published executable not found under {binDir}");
    }

    static async Task<(int exitCode, string stdout, string stderr)> RunExecutable(string exePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = exePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo)!;

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var stdout = await outputTask;
        var stderr = await errorTask;

        return (process.ExitCode, stdout, stderr);
    }
}

#endif
