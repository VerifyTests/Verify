static class GeneratorDriverRunResultExtensions
{
    public static (string HintName, string SourceText)? SelectGeneratedSources(this GeneratorDriverRunResult runResult) =>
        runResult
            .Results
            .SelectMany(_ => _.GeneratedSources)
            .Select(_ => (_.HintName, _.SourceText.ToString()))
            .SingleOrDefault();

    public static Dictionary<string, ImmutableArray<IncrementalGeneratorRunStep>> GetTrackedSteps(this GeneratorDriverRunResult runResult, IReadOnlyCollection<string> trackingNames) =>
        runResult
            .Results
            .SelectMany(_ => _.TrackedSteps)
            .Where(_ => trackingNames.Contains(_.Key))
            .ToDictionary(_ => _.Key, _ => _.Value);
}