using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VerifyMSTest.SourceGenerator;

static class Parser
{
    public static string MarkerAttributeName { get; } = "VerifyMSTest.UsesVerifyAttribute";

    public static ClassToGenerate Parse(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax typeSyntax)
    {
        var ns = GetNamespace(typeSymbol);
        var name = GetTypeNameWithGenericParameters(typeSyntax);
        var parents = GetParentClasses(typeSyntax);

        return new ClassToGenerate(ns, name, parents);
    }

    private static string? GetNamespace(INamedTypeSymbol symbol) =>
        symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToString();

    private static ParentClass[] GetParentClasses(TypeDeclarationSyntax typeSyntax)
    {
        // We can only be nested in class/struct/record
        static bool IsAllowedKind(SyntaxKind kind) =>
            kind == SyntaxKind.ClassDeclaration ||
            kind == SyntaxKind.StructDeclaration ||
            kind == SyntaxKind.RecordDeclaration;

        var parents = new Stack<ParentClass>();

        // Try and get the parent syntax. If it isn't a type like class/struct, this will be null
        var parentSyntax = typeSyntax.Parent as TypeDeclarationSyntax;

        // Keep looping while we're in a supported nested type
        while (parentSyntax != null && IsAllowedKind(parentSyntax.Kind()))
        {
            // Record the parent type keyword (class/struct etc), name, and constraints
            parents.Push(new ParentClass(
                keyword: parentSyntax.Keyword.ValueText,
                name: GetTypeNameWithGenericParameters(parentSyntax)));

            // Move to the next outer type
            parentSyntax = parentSyntax.Parent as TypeDeclarationSyntax;
        }

        return parents.ToArray();
    }

    private static string GetTypeNameWithGenericParameters(TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Identifier.ToString() + typeSyntax.TypeParameterList;
}