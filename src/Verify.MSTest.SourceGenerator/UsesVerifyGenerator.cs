using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace VerifyMSTest.SourceGenerator;

[Generator]
public class UsesVerifyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: Parser.MarkerAttributeName,
                predicate: IsSyntaxEligibleForGeneration,
                transform: GetSemanticTargetForGeneration)
            .WithTrackingName(TrackingNames.InitialTransform)
            .Where(static classToGenerate => classToGenerate is not null)
            .WithTrackingName(TrackingNames.RemoveNulls);

        // Collect the classes to generate into a collection so that we can write them
        // to a single file and avoid the issues of ambiguous hint names discussed in
        // https://github.com/dotnet/roslyn/discussions/60272.
        var classesCollection = classesToGenerate
            .Collect()
            .WithTrackingName(TrackingNames.Collect);

        context.RegisterSourceOutput(classesCollection, Execute);
    }

    private static bool IsSyntaxEligibleForGeneration(SyntaxNode node, CancellationToken ct) => node is ClassDeclarationSyntax;

    private static ClassToGenerate? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        if (context.TargetSymbol is not INamedTypeSymbol typeSymbol)
        {
            return null;
        }

        if (context.TargetNode is not TypeDeclarationSyntax typeSyntax)
        {
            return null;
        }

        ct.ThrowIfCancellationRequested();

        return Parser.Parse(typeSymbol, typeSyntax);
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<ClassToGenerate?> classesToGenerate)
    {
        if (classesToGenerate.IsDefaultOrEmpty)
        {
            return;
        }

        var classes = classesToGenerate.Distinct().OfType<ClassToGenerate>().ToArray();
        if (classes.Length == 0)
        {
            return;
        }

        var sourceCode = Emitter.GenerateExtensionClasses(classes);
        context.AddSource("UsesVerify.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}
