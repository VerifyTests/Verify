using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using VerifyMSTest;

namespace Verify.MSTest.SourceGenerator.Tests;

static class TestDriver
{
    public static GeneratorDriverRunResult Run(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        IReadOnlyCollection<PortableExecutableReference> references =
        [
            MetadataReference.CreateFromFile(typeof(VerifyMSTest.UsesVerifyAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        ];

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: [syntaxTree],
            references: references);

        var generator = new UsesVerifyGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        return driver.GetRunResult();
    }
}
