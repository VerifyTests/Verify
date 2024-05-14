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
}