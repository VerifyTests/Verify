static class Extensions
{
    public static string GetTypeNameWithGenericParameters(this TypeDeclarationSyntax syntax) =>
        syntax.Identifier.ToString() + syntax.TypeParameterList;

    public static IEnumerable<INamedTypeSymbol> GetBaseTypes(this ITypeSymbol symbol)
    {
        var baseType = symbol.BaseType;

        while (baseType is not null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    public static IncrementalValuesProvider<TSource> WhereNotNull<TSource>(this IncrementalValuesProvider<TSource?> source) where TSource : struct =>
        source
            .Where(_ => _.HasValue)
            .Select((item, _) => item!.Value);

    public static bool HasAttributeOfType(this ISymbol symbol, string fullyQualifiedAttributeName, bool includeDerived)
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

                if (includeDerived)
                {
                    typeSymbol = typeSymbol.BaseType;
                    continue;
                }

                typeSymbol = null;
            }
        }

        return false;
    }

    public static string? GetNamespaceOrDefault(this ISymbol symbol)
    {
        var ns = symbol.ContainingNamespace;
        if (ns.IsGlobalNamespace)
        {
            return null;
        }

        return ns.ToString();
    }
}
