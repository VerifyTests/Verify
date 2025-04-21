using PropertyFlags = ClassToGenerate.PropertyFlags;

static class Parser
{
    public static ClassToGenerate Parse(INamedTypeSymbol symbol, TypeDeclarationSyntax syntax, Cancel cancel)
    {
        cancel.ThrowIfCancellationRequested();
        var ns = symbol.GetNamespaceOrDefault();
        var name = syntax.GetTypeNameWithGenericParameters();
        var parents = GetParentClasses(syntax, cancel);
        var propertyFlags =
            GetDerivedPropertyFlagsGiven(
                BaseTestContextProperties(symbol).FirstOrDefault());

        return new(
            Namespace: ns,
            ClassName: name,
            TestContextPropertyFlags: propertyFlags,
            ParentClasses: parents);
    }

    static PropertyFlags GetDerivedPropertyFlagsGiven(IPropertySymbol? baseClassProperty) =>
        baseClassProperty?.IsAbstract switch
        {
            true => PropertyFlags.Override,
            false => PropertyFlags.Override |
                     PropertyFlags.CallBase,
            null => PropertyFlags.None
        };

    static IEnumerable<IPropertySymbol> BaseTestContextProperties(INamedTypeSymbol typeSymbol) =>
        BaseClassesOf(typeSymbol)
            .SelectMany(GetTestContextProperty);

    static IEnumerable<IPropertySymbol> GetTestContextProperty(INamedTypeSymbol typeSymbol) =>
        typeSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(_ =>
                    _.Name == "TestContext" &&
                    _.DeclaredAccessibility == Accessibility.Public);

    static IEnumerable<INamedTypeSymbol> BaseClassesOf(INamedTypeSymbol typeSymbol)
    {
        var baseType = typeSymbol.BaseType;
        while (baseType?.TypeKind == TypeKind.Class)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    static ParentClass[] GetParentClasses(TypeDeclarationSyntax syntax, Cancel cancel)
    {
        // We can only be nested in class/struct/record
        static bool IsAllowedKind(SyntaxKind kind) =>
            kind is
                SyntaxKind.ClassDeclaration or
                SyntaxKind.StructDeclaration or
                SyntaxKind.RecordDeclaration;

        var parents = new Stack<ParentClass>();

        var parent = syntax.Parent as TypeDeclarationSyntax;

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