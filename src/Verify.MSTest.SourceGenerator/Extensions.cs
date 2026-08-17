static class Extensions
{
    public static string GetTypeNameWithGenericParameters(this TypeDeclarationSyntax syntax) =>
        syntax.Identifier.ToString() + syntax.TypeParameterList;

    /// <summary>
    /// The keyword to redeclare the type with. A partial declaration has to repeat the kind,
    /// and for a record struct that kind is two tokens: `Keyword` is only the `record` half.
    /// </summary>
    public static string GetPartialKeyword(this TypeDeclarationSyntax syntax)
    {
        if (syntax is RecordDeclarationSyntax record &&
            record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword))
        {
            return "record struct";
        }

        return syntax.Keyword.ValueText;
    }

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

    public static bool HasAttributeOfType(this ISymbol symbol, INamedTypeSymbol attributeType, bool includeDerived)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            var type = attribute.AttributeClass;
            while (type is not null)
            {
                if (SymbolEqualityComparer.Default.Equals(type, attributeType))
                {
                    return true;
                }

                if (includeDerived)
                {
                    type = type.BaseType;
                    continue;
                }

                type = null;
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
