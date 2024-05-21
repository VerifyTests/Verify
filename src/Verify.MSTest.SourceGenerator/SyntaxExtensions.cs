static class SyntaxExtensions
{
    public static string GetTypeNameWithGenericParameters(this TypeDeclarationSyntax typeSyntax) =>
        typeSyntax.Identifier.ToString() + typeSyntax.TypeParameterList;
}
