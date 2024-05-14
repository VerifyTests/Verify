using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VerifyMSTest.SourceGenerator;

static class Parser
{
    public static string MarkerAttributeName { get; } = "VerifyMSTest.UsesVerifyAttribute";

    public static ClassToGenerate? Parse(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax typeSyntax)
    {
        // Only generate for classes that don't already have a TestContext property defined.
        if (HasTestContextProperty(typeSymbol))
        {
            return null;
        }

        var ns = GetNamespace(typeSymbol);
        var name = GetTypeNameWithGenericParameters(typeSyntax);
        var parents = GetParentClasses(typeSyntax);

        return new ClassToGenerate(ns, name, parents);
    }

    private static string? GetNamespace(INamedTypeSymbol symbol) =>
        symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToString();

    private static bool HasTestContextProperty(INamedTypeSymbol symbol) =>
        HasMarkerAttributeOnBase(symbol) || HasTestContextPropertyDefinedInBase(symbol);

    private static bool HasMarkerAttributeOnBase(INamedTypeSymbol symbol)
    {
        static bool HasMarkerAttribute(ISymbol symbol) =>
            symbol
            .GetAttributes()
            .Any(a => a.AttributeClass?.ToDisplayString() == MarkerAttributeName);

        var parent = symbol.BaseType;

        while (parent is not null)
        {
            if (HasMarkerAttribute(parent))
            {
                return true;
            }

            parent = parent.BaseType;
        }

        return false;
    }

    private static bool HasTestContextPropertyDefinedInBase(INamedTypeSymbol symbol)
    {
        static bool HasTestContextProperty(INamedTypeSymbol symbol) =>
            symbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Any(p => p.Name == "TestContext" && p.Type.Name == "TestContext");

        var parent = symbol.BaseType;

        while (parent is not null)
        {
            if (HasTestContextProperty(parent))
            {
                return true;
            }

            parent = parent.BaseType;
        }

        return false;
    }

    private static ParentClass[] GetParentClasses(TypeDeclarationSyntax typeSyntax)
    {
        // We can only be nested in class/struct/record
        static bool IsAllowedKind(SyntaxKind kind) =>
            kind == SyntaxKind.ClassDeclaration ||
            kind == SyntaxKind.StructDeclaration ||
            kind == SyntaxKind.RecordDeclaration;

        var parents = new Stack<ParentClass>();

        var parentSyntax = typeSyntax.Parent as TypeDeclarationSyntax;

        while (parentSyntax is not null && IsAllowedKind(parentSyntax.Kind()))
        {
            parents.Push(new ParentClass(
                keyword: parentSyntax.Keyword.ValueText,
                name: GetTypeNameWithGenericParameters(parentSyntax)));

            parentSyntax = parentSyntax.Parent as TypeDeclarationSyntax;
        }

        return parents.ToArray();
    }

    private static string GetTypeNameWithGenericParameters(TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Identifier.ToString() + typeSyntax.TypeParameterList;
}