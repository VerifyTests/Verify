static class SymbolExtensions
{
    public static bool HasAttribute(this ISymbol symbol, string attributeName) =>
        symbol
        .GetAttributes()
        .Any(attribute => attribute.AttributeClass?.ToDisplayString() == attributeName);

    public static bool HasAttributeOnBaseTypes(this ITypeSymbol symbol, string attributeName)
    {
        var parent = symbol.BaseType;

        while (parent is not null)
        {
            if (parent.HasAttribute(attributeName))
            {
                return true;
            }

            parent = parent.BaseType;
        }

        return false;
    }

    public static bool HasAttributeThatInheritsFrom(this ISymbol symbol, string attributeName)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            var typeSymbol = attribute.AttributeClass;
            while (typeSymbol is not null)
            {
                if (typeSymbol.ToDisplayString() == attributeName)
                {
                    return true;
                }

                typeSymbol = typeSymbol.BaseType;
            }
        }

        return false;
    }

    public static string? GetNamespaceOrDefault(this ISymbol symbol) =>
        symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToString();
}
