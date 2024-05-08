using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Verify.MSTest.SourceGenerator;

[Generator]
public class UsesVerifyGenerator : IIncrementalGenerator
{
    private static string MarkerAttributeName { get; } = "VerifyMSTest.UsesVerifyAttribute";
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classesToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: MarkerAttributeName,
                predicate: IsSyntaxEligibleForGeneration,
                transform: GetSemanticTargetForGeneration)
            .WithTrackingName(TrackingNames.InitialTransform)
            .Where(static classToGenerate => classToGenerate is not null)
            .WithTrackingName(TrackingNames.RemovingNulls);

        context.RegisterSourceOutput(classesToGenerate, Execute);
    }

    private static bool IsSyntaxEligibleForGeneration(SyntaxNode node, CancellationToken ct) => node is ClassDeclarationSyntax;

    static ClassToGenerate? GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        if (context.TargetSymbol is not INamedTypeSymbol classSymbol)
        {
            return null;
        }

        ct.ThrowIfCancellationRequested();

        return new ClassToGenerate(classSymbol.ContainingNamespace.ToDisplayString(), classSymbol.Name);
    }

    private static void Execute(SourceProductionContext context, ClassToGenerate? classToGenerate)
    {
        if (classToGenerate is { } value)
        {
            var sourceCode = CodeTemplates.GenerateExtensionClass(value);

            context.AddSource($"UsesVerify.{value.ClassName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }
}
