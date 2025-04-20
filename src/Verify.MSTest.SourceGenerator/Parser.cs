using System.Data;
using PropertyFlags = ClassToGenerate.PropertyFlags;

static class Parser
{
    public static ClassToGenerate? Parse(INamedTypeSymbol typeSymbol, TypeDeclarationSyntax typeSyntax, Cancel cancel)
    {
        var ns = typeSymbol.GetNamespaceOrDefault();
        var name = typeSyntax.GetTypeNameWithGenericParameters();
        var parents = GetParentClasses(typeSyntax, cancel);
        var testContextPropertyFlags =
            GetDerivedPropertyFlagsGiven(
                BaseTestContextProperties(typeSymbol).FirstOrDefault());

        return new ClassToGenerate(
            Namespace: ns,
            ClassName: name,
            TestContextPropertyFlags: testContextPropertyFlags,
            ParentClasses: parents);
    }

    private static PropertyFlags GetDerivedPropertyFlagsGiven(
        IPropertySymbol? baseClassProperty)
    {
        var isAbstract = baseClassProperty?.IsAbstract;
        switch (isAbstract)
        {
            case true:
                return PropertyFlags.Override;
            case false:
                return PropertyFlags.Override | PropertyFlags.CallBase;
            case null:
                return PropertyFlags.None;
        }
    }

    static IEnumerable<IPropertySymbol> BaseTestContextProperties(
        INamedTypeSymbol typeSymbol)
    =>
            from baseClass in BaseClassesOf(typeSymbol)
            from testContextProperty in GetTestContextProperty(baseClass)
            select testContextProperty;


    static IEnumerable<IPropertySymbol> GetTestContextProperty(INamedTypeSymbol typeSymbol) =>
        typeSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(property =>
                    property.Name == "TestContext" &&
                    property.DeclaredAccessibility == Accessibility.Public);

    static IEnumerable<INamedTypeSymbol> BaseClassesOf(INamedTypeSymbol typeSymbol)
    {
        var baseType = typeSymbol.BaseType;
        while (baseType?.TypeKind == TypeKind.Class)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
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