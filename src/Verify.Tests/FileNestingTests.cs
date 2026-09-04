#if NET10_0

using System.Text.Json;

// Snapshot nesting is applied during evaluation, because evaluated items are what Solution Explorer
// reads. So these tests evaluate a project and inspect its items, rather than building it. The one
// target they run is the SDK's duplicate item check, since that is what rejects a build where the
// nesting has left a snapshot included twice.
public class FileNestingTests
{
    [Fact]
    public async Task CSharpProject() =>
        await Verify(
            await Evaluate(
                CSharp("Microsoft.NET.Sdk"),
                "Tests.cs",
                "Tests.Simple.verified.txt",
                // outside the razor and web SDKs, json snapshots are None like any other extension
                "Tests.Simple.verified.json",
                "Tests.Simple.received.txt",
                // a snapshot copied to the intermediate directory is not part of the project
                "obj/Debug/net10.0/Tests.Simple.verified.txt"));

    [Fact]
    public async Task RazorProject() =>
        await Verify(
            await Evaluate(
                CSharp("Microsoft.NET.Sdk.Razor"),
                // a component with a code behind: snapshots nest under the code behind
                "ComponentTests.razor",
                "ComponentTests.razor.cs",
                "ComponentTests.Simple.verified.html",
                // the razor and web SDKs claim json as Content rather than None
                "ComponentTests.Simple.verified.json",
                // a component without a code behind: the snapshot nests under the razor file
                "NoCodeBehindTests.razor",
                "NoCodeBehindTests.Simple.verified.html",
                // a component in a sub directory
                "Sub/SubComponentTests.razor",
                "Sub/SubComponentTests.razor.cs",
                "Sub/SubComponentTests.Simple.verified.html",
                // a plain test class in a razor project still nests under the cs file
                "PlainTests.cs",
                "PlainTests.Simple.verified.txt"));

    // A test class can sit in a code behind whose component lives in the project under test, so the
    // project holding the test has the .razor.cs but not the .razor.
    [Fact]
    public async Task ProjectWithOnlyACodeBehind() =>
        await Verify(
            await Evaluate(
                CSharp("Microsoft.NET.Sdk"),
                "CodeBehindTests.razor.cs",
                "CodeBehindTests.Component.verified.html",
                "CodeBehindTests.Component.verified.txt"));

    // https://github.com/VerifyTests/Verify.Bunit/issues/108#issuecomment-5522040465
    // A build fails with NETSDK1022 when a snapshot is a Content item twice. The razor SDK leaves
    // html snapshots as None, so a project holding them as Content has declared that itself, and
    // the nesting has to update those items rather than add to them.
    [Fact]
    public async Task RazorProjectWithSnapshotsAsContent() =>
        await Verify(
            await Evaluate(
                CSharp(
                    "Microsoft.NET.Sdk.Razor",
                    body:
                    """
                      <ItemGroup>
                        <None Remove="**\*.verified.html" />
                        <Content Include="**\*.verified.html" />
                      </ItemGroup>
                    """),
                "ComponentTests.razor",
                "ComponentTests.razor.cs",
                "ComponentTests.Simple.verified.html",
                "ComponentTests.Simple.verified.json"));

    [Fact]
    public async Task RazorProjectWithNestingDisabled() =>
        await Verify(
            await Evaluate(
                CSharp(
                    "Microsoft.NET.Sdk.Razor",
                    properties: "<DisableVerifyFileNesting>true</DisableVerifyFileNesting>"),
                "ComponentTests.razor",
                "ComponentTests.razor.cs",
                "ComponentTests.Simple.verified.html",
                "ComponentTests.Simple.verified.json"));

    [Fact]
    public async Task WebProject() =>
        await Verify(
            await Evaluate(
                CSharp(
                    "Microsoft.NET.Sdk.Web",
                    body:
                    """
                      <ItemGroup>
                        <Content Update="Tests.Explicit.verified.json" DependentUpon="Other.cs" />
                      </ItemGroup>
                    """),
                "Tests.cs",
                "Other.cs",
                // a web project has no razor files, so no parent is probed for
                "Tests.Simple.verified.json",
                "Tests.Simple.verified.config",
                "Tests.Simple.verified.txt",
                // an explicitly nested snapshot keeps the parent it was given
                "Tests.Explicit.verified.json"));

    [Fact]
    public async Task VisualBasicProject() =>
        await Verify(
            await Evaluate(
                VisualBasic(),
                "Tests.vb",
                "Tests.Simple.verified.txt"));

    static (string file, string content) CSharp(string sdk, string properties = "", string body = "") =>
        ("TestProject.csproj", ProjectContent(sdk, properties, body));

    static (string file, string content) VisualBasic() =>
        ("TestProject.vbproj", ProjectContent("Microsoft.NET.Sdk", "", ""));

    static string ProjectContent(string sdk, string properties, string body) =>
        $"""
         <Project Sdk="{sdk}">
           <PropertyGroup>
             <TargetFramework>net10.0</TargetFramework>
             {properties}
           </PropertyGroup>
         {body}
         </Project>
         """;

    static async Task<string> Evaluate((string file, string content) project, params string[] files)
    {
        using var directory = new TempDirectory();
        var path = directory.Path;

        // The package imports these two through the buildTransitive convention, which lands them
        // either side of the SDK. Directory.Build.props and .targets are those same two points.
        await WriteFile(path, "Directory.Build.props", Import("Verify.props"));
        await WriteFile(path, "Directory.Build.targets", Import("Verify.targets"));
        await WriteFile(path, project.file, project.content);

        foreach (var file in files)
        {
            await WriteFile(path, file, "");
        }

        return Format(await RunMsBuild(Path.Combine(path, project.file)));
    }

    static string Import(string file)
    {
        var full = Path.Combine(ProjectFiles.SolutionDirectory, "Verify", "buildTransitive", file);
        return $"""
                <Project>
                  <Import Project="{full}" />
                </Project>
                """;
    }

    static async Task WriteFile(string directory, string relativePath, string content)
    {
        var path = Path.Combine(directory, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await File.WriteAllTextAsync(path, content);
    }

    static string Format(string json)
    {
        using var document = JsonDocument.Parse(json);
        var items = document.RootElement.GetProperty("Items");
        var builder = new StringBuilder();
        string[] types = ["None", "Content"];
        foreach (var type in types)
        {
            builder.AppendLine(type);

            var snapshots = Snapshots(items, type);
            if (snapshots.Count == 0)
            {
                builder.AppendLine("  none");
                continue;
            }

            foreach (var snapshot in snapshots)
            {
                builder.AppendLine($"  {snapshot.Key}: {Parent(snapshot)}");
            }
        }

        return builder.ToString();
    }

    // Verify adds its own item for a snapshot on top of the one the SDK globbed, so the same file
    // can appear more than once. Any disagreement between those items is a bug.
    static string Parent(IEnumerable<(string Identity, string? Parent)> snapshot)
    {
        var parents = snapshot
            .Select(_ => _.Parent)
            .Where(_ => _ != null)
            .Distinct()
            .ToList();
        if (parents.Count == 0)
        {
            return "not nested";
        }

        return string.Join(" and ", parents);
    }

    static List<IGrouping<string, (string Identity, string? Parent)>> Snapshots(JsonElement items, string type)
    {
        if (!items.TryGetProperty(type, out var ofType))
        {
            return [];
        }

        return ofType
            .EnumerateArray()
            .Select(_ => (
                // MSBuild uses the platform separator, and these paths are part of the snapshot
                Identity: _.GetProperty("Identity").GetString()!.Replace('\\', '/'),
                Parent: _.TryGetProperty("DependentUpon", out var parent) ? parent.GetString() : null))
            .Where(_ => _.Identity.Contains(".verified.") ||
                        _.Identity.Contains(".received."))
            .GroupBy(_ => _.Identity)
            .OrderBy(_ => _.Key, StringComparer.Ordinal)
            .ToList();
    }

    static async Task<string> RunMsBuild(string projectPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var arguments = startInfo.ArgumentList;
        arguments.Add("msbuild");
        arguments.Add(projectPath);
        // The SDK fails a build with NETSDK1022 when the same file is a Content item twice, which an
        // evaluation alone never surfaces. So the target that performs that check runs before the
        // items are read, and a duplicate fails the test with the error a build would give.
        arguments.Add("-t:CheckForDuplicateItems");
        arguments.Add("-getItem:None");
        arguments.Add("-getItem:Content");
        arguments.Add("-nologo");

        using var process = Process.Start(startInfo)!;

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var output = await outputTask;
        var error = await errorTask;

        if (process.ExitCode == 0 &&
            output.TrimStart().StartsWith('{'))
        {
            return output;
        }

        throw new($"Evaluation of {projectPath} failed:{Environment.NewLine}{error}{Environment.NewLine}{output}");
    }
}

#endif
