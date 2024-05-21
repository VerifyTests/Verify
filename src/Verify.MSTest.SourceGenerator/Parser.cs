static class Parser
{
    public static string MarkerAttributeName => "VerifyMSTest.UsesVerifyAttribute";

    public static ClassToGenerate? Parse(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax typeSyntax, Cancel cancel)
    {
        // Only generate for classes that won't get one defined by another attribute.
        if (typeSymbol.HasAttributeOnBaseTypes(MarkerAttributeName))
        {
            return null;
        }

        var ns = typeSymbol.GetNamespaceOrDefault();
        var name = typeSyntax.GetTypeNameWithGenericParameters();
        var parents = GetParentClasses(typeSyntax, cancel);

        return new ClassToGenerate(ns, name, parents);
    }

    static ParentClass[] GetParentClasses(TypeDeclarationSyntax typeSyntax, Cancel cancel)
    {
        // We can only be nested in class/struct/record
        static bool IsAllowedKind(SyntaxKind kind) =>
            kind is
                SyntaxKind.ClassDeclaration or
                SyntaxKind.StructDeclaration or
                SyntaxKind.RecordDeclaration;

        var parents = new Stack<ParentClass>();

        var parentSyntax = typeSyntax.Parent as TypeDeclarationSyntax;

        while (parentSyntax is not null &&
               IsAllowedKind(parentSyntax.Kind()))
        {
            cancel.ThrowIfCancellationRequested();

            parents.Push(new(
                Keyword: parentSyntax.Keyword.ValueText,
                Name: parentSyntax.GetTypeNameWithGenericParameters()));

            parentSyntax = parentSyntax.Parent as TypeDeclarationSyntax;
        }

        return parents.ToArray();
    }
}