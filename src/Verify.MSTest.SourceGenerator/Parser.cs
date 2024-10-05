static class Parser
{
    public static ClassToGenerate? Parse(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax typeSyntax, Cancel cancel)
    {
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

        var parent = typeSyntax.Parent as TypeDeclarationSyntax;

        while (parent is not null &&
               IsAllowedKind(parent.Kind()))
        {
            cancel.ThrowIfCancellationRequested();

            parents.Push(new(
                Keyword: parent.Keyword.ValueText,
                Name: parent.GetTypeNameWithGenericParameters()));

            parent = parent.Parent as TypeDeclarationSyntax;
        }

        return parents.ToArray();
    }
}