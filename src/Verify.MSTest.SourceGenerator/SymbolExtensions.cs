static class SymbolExtensions
{
    public static bool HasAttributeOnBaseTypes(this INamedTypeSymbol symbol, string attributeName)
    {
        static bool HasAttribute(INamedTypeSymbol symbol, string attributeName) =>
            symbol
            .GetAttributes()
            .Any(_ => _.AttributeClass?.ToDisplayString() == attributeName);

        var parent = symbol.BaseType;

        while (parent is not null)
        {
            if (HasAttribute(parent, attributeName))
            {
                return true;
            }

            parent = parent.BaseType;
        }

        return false;
    }

    public static string? GetNamespaceOrDefault(this INamedTypeSymbol symbol) =>
        symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToString();
}
