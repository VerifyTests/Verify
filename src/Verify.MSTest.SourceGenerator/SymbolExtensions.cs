static class SymbolExtensions
{
    public static IEnumerable<INamedTypeSymbol> GetBaseTypes(this ITypeSymbol? symbol)
    {
        var baseType = symbol?.BaseType;

        while (baseType is not null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    public static bool HasAttributeOnBaseTypes(this ITypeSymbol symbol, string attributeName) =>
        symbol
        .GetBaseTypes()
        .Any(x => x.HasAttributeOfType(attributeName, allowInheritance: false));

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
