static class SymbolExtensions
{
    public static bool HasAttributeOnBaseTypes(this ITypeSymbol symbol, string attributeName)
    {
        var parent = symbol.BaseType;

        while (parent is not null)
        {
            if (parent.HasAttributeOfType(attributeName, allowInheritance: false))
            {
                return true;
            }

            parent = parent.BaseType;
        }

        return false;
    }

    public static bool HasAttributeOfType(this ISymbol symbol, string fullyQualifiedAttributeName, bool allowInheritance)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            var typeSymbol = attribute.AttributeClass;
            while (typeSymbol is not null)
            {
                if (typeSymbol.ToDisplayString() == fullyQualifiedAttributeName)
                {
                    return true;
                }

                typeSymbol = allowInheritance ? typeSymbol.BaseType : null;
            }
        }

        return false;
    }

    public static string? GetNamespaceOrDefault(this ISymbol symbol) =>
        symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToString();
}
