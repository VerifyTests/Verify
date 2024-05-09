using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        if (context.TargetNode is not BaseTypeDeclarationSyntax declarationSyntax)
        {
            return null;
        }

        ct.ThrowIfCancellationRequested();

        var @namespace = classSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : classSymbol.ContainingNamespace.ToString();
        var typeParameters = classSymbol.TypeParameters.Select(tp => tp.Name).ToArray(); // TODO: May be able to use Syntax instead
        var parentClass = GetParentClasses(declarationSyntax);

        return new ClassToGenerate(@namespace, classSymbol.Name, typeParameters, parentClass);
    }

    private static void Execute(SourceProductionContext context, ClassToGenerate? classToGenerate)
    {
        if (classToGenerate is { } value)
        {
            var sourceCode = CodeWriter.GenerateExtensionClass(value);

            context.AddSource($"UsesVerify.{value.ClassName}.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    private static ParentClass? GetParentClasses(BaseTypeDeclarationSyntax typeSyntax)
    {
        // TODO: Redo as stack

        // We can only be nested in class/struct/record
        static bool IsAllowedKind(SyntaxKind kind) =>
            kind == SyntaxKind.ClassDeclaration ||
            kind == SyntaxKind.StructDeclaration ||
            kind == SyntaxKind.RecordDeclaration;

        // Try and get the parent syntax. If it isn't a type like class/struct, this will be null
        var parentSyntax = typeSyntax.Parent as TypeDeclarationSyntax;
        ParentClass? parentClassInfo = null;

        // Keep looping while we're in a supported nested type
        while (parentSyntax != null && IsAllowedKind(parentSyntax.Kind()))
        {
            // Record the parent type keyword (class/struct etc), name, and constraints
            parentClassInfo = new ParentClass(
                keyword: parentSyntax.Keyword.ValueText,
                name: parentSyntax.Identifier.ToString() + parentSyntax.TypeParameterList,
                constraints: parentSyntax.ConstraintClauses.ToString(), // TODO: I think I can remove this
                child: parentClassInfo);

            // Move to the next outer type
            parentSyntax = (parentSyntax.Parent as TypeDeclarationSyntax);
        }

        return parentClassInfo;

    }
}
