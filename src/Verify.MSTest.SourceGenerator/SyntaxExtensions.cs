static class SyntaxExtensions
{
    public static string GetTypeNameWithGenericParameters(this TypeDeclarationSyntax syntax) =>
        syntax.Identifier.ToString() + syntax.TypeParameterList;
}
