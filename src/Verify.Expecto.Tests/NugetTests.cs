#if RELEASE && NET9_0

public class NugetTests
{
    [Tests]
    public static Test nuget = Runner.TestCase(
        nameof(nuget),
        () =>
        {
            var version = typeof(NugetTests).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
                .InformationalVersion.Split('+')
                .First();
            var nugetPath = Path.Combine(
                AttributeReader.GetSolutionDirectory(),
                $"../nugets/Verify.Expecto.{version}.nupkg");
            var settings = new VerifySettings();
            settings.ScrubLinesContaining("psmdcp", "branch");
            settings.ScrubLinesWithReplace(_ => _.Replace(version, "version"));
            return VerifyZip(
                    name: nameof(nuget),
                    nugetPath,
                    include: _ =>
                    {
                        var extension = Path.GetExtension(_.Name);
                        return !extension.Contains(".psmdc") &&
                               !extension.Contains(".xml") &&
                               !extension.Contains(".dll") &&
                               !extension.Contains(".rels");
                    },
                    includeStructure: true,
                    settings: settings);
        });
}
#endif