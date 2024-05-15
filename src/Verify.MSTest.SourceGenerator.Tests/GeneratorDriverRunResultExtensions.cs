using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace VerifyMSTest.SourceGenerator.Tests;

static class GeneratorDriverRunResultExtensions
{
    public static (string HintName, string SourceText)? SelectGeneratedSources(this GeneratorDriverRunResult gdrr) =>
        gdrr
            .Results
            .SelectMany(grr => grr.GeneratedSources)
            .Select(gs => (gs.HintName, gs.SourceText.ToString()))
            .SingleOrDefault();

    public static Dictionary<string, ImmutableArray<IncrementalGeneratorRunStep>> GetTrackedSteps(this GeneratorDriverRunResult runResult, IReadOnlyCollection<string> trackingNames) =>
        runResult
            .Results
            .SelectMany(result => result.TrackedSteps)
            .Where(step => trackingNames.Contains(step.Key))
            .ToDictionary(x => x.Key, x => x.Value);
}